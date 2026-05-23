namespace Infrastructure.Ef;

using Domain.Dto;
using Microsoft.EntityFrameworkCore;

public class EfSearchRecetas(AppDbContext db) : ISearchRecetas
{
    public async Task<List<RecetaDto>> Process(string query, int? limit = 20)
    {
        var normalized = query.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return [];

        var take = Math.Clamp(limit ?? 20, 1, 100);

        return await db.Recetas
            .AsNoTracking()
            .Where(r => r.Nombre.Contains(normalized))
            .OrderBy(r => r.Nombre)
            .Take(take)
            .Select(r => new RecetaDto(r.Id, r.Nombre, r.ImagenUrl, r.Nivel, r.Url))
            .ToListAsync();
    }
}
