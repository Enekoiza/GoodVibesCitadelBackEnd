namespace Infrastructure.Ef;

using Domain.Dto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class EfSearchWarehouseCatalog(AppDbContext db) : ISearchWarehouseCatalog
{
    public async Task<List<WarehouseCatalogResultDto>> Process(string entryType, string query, int? limit = 20)
    {
        if (!EfAddCpWarehouseEntry.TryParseEntryType(entryType, out var type))
        {
            return [];
        }

        var normalized = query.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return [];
        }

        var take = Math.Clamp(limit ?? 20, 1, 100);

        return type switch
        {
            WarehouseEntryType.Item => await SearchItems(normalized, take),
            WarehouseEntryType.Material => await SearchMaterials(normalized, take),
            WarehouseEntryType.Receta => await SearchRecetas(normalized, take),
            _ => [],
        };
    }

    public async Task<List<WarehouseCatalogResultDto>> ProcessAll(string query, int? limit = 20)
    {
        var normalized = query.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return [];
        }

        var take = Math.Clamp(limit ?? 20, 1, 100);

        // DbContext is not thread-safe — queries must run sequentially.
        var items = await SearchItems(normalized, take);
        var materials = await SearchMaterials(normalized, take);
        var recetas = await SearchRecetas(normalized, take);

        return items
            .Concat(materials)
            .Concat(recetas)
            .OrderBy(result => result.Nombre)
            .Take(take)
            .ToList();
    }

    private Task<List<WarehouseCatalogResultDto>> SearchItems(string normalized, int take) =>
        db.Items
            .AsNoTracking()
            .Where(item => item.Nombre.ToLower().Contains(normalized.ToLower()))
            .OrderBy(item => item.Nombre)
            .Take(take)
            .Select(item => new WarehouseCatalogResultDto(item.Id, item.Nombre, item.ImagenUrl, "Item"))
            .ToListAsync();

    private Task<List<WarehouseCatalogResultDto>> SearchMaterials(string normalized, int take) =>
        db.Materiales
            .AsNoTracking()
            .Where(material => material.Nombre.ToLower().Contains(normalized.ToLower()))
            .OrderBy(material => material.Nombre)
            .Take(take)
            .Select(material => new WarehouseCatalogResultDto(material.Id, material.Nombre, material.ImagenUrl, "Material"))
            .ToListAsync();

    private Task<List<WarehouseCatalogResultDto>> SearchRecetas(string normalized, int take) =>
        db.Recetas
            .AsNoTracking()
            .Where(receta => receta.Nombre.ToLower().Contains(normalized.ToLower()))
            .OrderBy(receta => receta.Nombre)
            .Take(take)
            .Select(receta => new WarehouseCatalogResultDto(receta.Id, receta.Nombre, receta.ImagenUrl, "Receta"))
            .ToListAsync();
}
