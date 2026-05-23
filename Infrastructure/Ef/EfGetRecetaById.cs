namespace Infrastructure.Ef;

using Domain.Dto;
using Microsoft.EntityFrameworkCore;

public class EfGetRecetaById(AppDbContext db) : IGetRecetaById
{
    public async Task<RecetaDetailDto?> Process(int id)
    {
        var receta = await db.Recetas
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receta is null)
            return null;

        var materiales = await db.RecetaMateriales
            .AsNoTracking()
            .Include(m => m.Material)
            .Where(m => m.RecetaId == id)
            .OrderBy(m => m.Id)
            .ToListAsync();

        return new RecetaDetailDto(
            receta.Id,
            receta.Nombre,
            receta.ImagenUrl,
            receta.Nivel,
            receta.Url,
            RecetaMaterialesTreeBuilder.Build(materiales));
    }
}
