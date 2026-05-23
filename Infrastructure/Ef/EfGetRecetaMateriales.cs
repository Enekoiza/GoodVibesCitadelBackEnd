namespace Infrastructure.Ef;

using Domain.Dto;
using Microsoft.EntityFrameworkCore;

public class EfGetRecetaMateriales(AppDbContext db) : IGetRecetaMateriales
{
    public async Task<RecetaMaterialesResponseDto?> Process(int recetaId)
    {
        var recetaExists = await db.Recetas
            .AsNoTracking()
            .AnyAsync(r => r.Id == recetaId);

        if (!recetaExists)
            return null;

        var materiales = await db.RecetaMateriales
            .AsNoTracking()
            .Include(m => m.Material)
            .Where(m => m.RecetaId == recetaId)
            .OrderBy(m => m.Id)
            .ToListAsync();

        var items = await (
            from itemReceta in db.ItemRecetas.AsNoTracking()
            join item in db.Items.AsNoTracking() on itemReceta.ItemId equals item.Id
            where itemReceta.RecetaId == recetaId
            orderby item.Nombre
            select new RecetaCraftedItemDto(item.Id, item.Nombre, item.ImagenUrl, item.Grado)
        ).ToListAsync();

        return new RecetaMaterialesResponseDto(
            RecetaMaterialesTreeBuilder.Build(materiales),
            items);
    }
}
