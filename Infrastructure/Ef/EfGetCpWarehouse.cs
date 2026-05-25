namespace Infrastructure.Ef;

using Domain.Dto;
using Domain.Entities;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

public class EfGetCpWarehouse(AppDbContext db) : IGetCpWarehouse
{
    public async Task<List<CpWarehouseEntryDto>> Process()
    {
        var entries = await db.CpWarehouseEntries
            .AsNoTracking()
            .OrderBy(e => e.EntryType)
            .ThenBy(e => e.Id)
            .ToListAsync();

        return await CpWarehouseEntryMapper.MapEntries(db, entries);
    }
}

internal static class CpWarehouseEntryMapper
{
    public static async Task<List<CpWarehouseEntryDto>> MapEntries(
        AppDbContext db,
        IReadOnlyList<CpWarehouseEntry> entries)
    {
        if (entries.Count == 0)
        {
            return [];
        }

        var itemIds = entries
            .Where(e => e.EntryType == WarehouseEntryType.Item)
            .Select(e => e.EntityId)
            .Distinct()
            .ToList();

        var materialIds = entries
            .Where(e => e.EntryType == WarehouseEntryType.Material)
            .Select(e => e.EntityId)
            .Distinct()
            .ToList();

        var recetaIds = entries
            .Where(e => e.EntryType == WarehouseEntryType.Receta)
            .Select(e => e.EntityId)
            .Distinct()
            .ToList();

        var items = itemIds.Count > 0
            ? await db.Items.AsNoTracking().Where(i => itemIds.Contains(i.Id)).ToDictionaryAsync(i => i.Id)
            : new Dictionary<int, Item>();

        var materials = materialIds.Count > 0
            ? await db.Materiales.AsNoTracking().Where(m => materialIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id)
            : new Dictionary<int, Material>();

        var recetas = recetaIds.Count > 0
            ? await db.Recetas.AsNoTracking().Where(r => recetaIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id)
            : new Dictionary<int, Receta>();

        var result = new List<CpWarehouseEntryDto>(entries.Count);

        foreach (var entry in entries)
        {
            var (nombre, imagenUrl) = ResolveCatalogEntry(entry, items, materials, recetas);
            result.Add(new CpWarehouseEntryDto(
                entry.Id,
                entry.EntryType.ToString(),
                entry.EntityId,
                nombre,
                imagenUrl,
                entry.Quantity));
        }

        return result;
    }

    public static async Task<ErrorOr<(string Nombre, string? ImagenUrl)>> ResolveCatalogEntry(
        AppDbContext db,
        WarehouseEntryType entryType,
        int entityId)
    {
        return entryType switch
        {
            WarehouseEntryType.Item => await ResolveItem(db, entityId),
            WarehouseEntryType.Material => await ResolveMaterial(db, entityId),
            WarehouseEntryType.Receta => await ResolveReceta(db, entityId),
            _ => Error.Validation(description: "Tipo de entrada no válido."),
        };
    }

    private static (string Nombre, string? ImagenUrl) ResolveCatalogEntry(
        CpWarehouseEntry entry,
        IReadOnlyDictionary<int, Item> items,
        IReadOnlyDictionary<int, Material> materials,
        IReadOnlyDictionary<int, Receta> recetas)
    {
        return entry.EntryType switch
        {
            WarehouseEntryType.Item when items.TryGetValue(entry.EntityId, out var item) =>
                (item.Nombre, item.ImagenUrl),
            WarehouseEntryType.Material when materials.TryGetValue(entry.EntityId, out var material) =>
                (material.Nombre, material.ImagenUrl),
            WarehouseEntryType.Receta when recetas.TryGetValue(entry.EntityId, out var receta) =>
                (receta.Nombre, receta.ImagenUrl),
            _ => ($"(eliminado #{entry.EntityId})", null),
        };
    }

    private static async Task<ErrorOr<(string Nombre, string? ImagenUrl)>> ResolveItem(AppDbContext db, int entityId)
    {
        var item = await db.Items.AsNoTracking().SingleOrDefaultAsync(i => i.Id == entityId);
        return item is null
            ? Error.NotFound(description: "No se encontró el item.")
            : (item.Nombre, item.ImagenUrl);
    }

    private static async Task<ErrorOr<(string Nombre, string? ImagenUrl)>> ResolveMaterial(AppDbContext db, int entityId)
    {
        var material = await db.Materiales.AsNoTracking().SingleOrDefaultAsync(m => m.Id == entityId);
        return material is null
            ? Error.NotFound(description: "No se encontró el material.")
            : (material.Nombre, material.ImagenUrl);
    }

    private static async Task<ErrorOr<(string Nombre, string? ImagenUrl)>> ResolveReceta(AppDbContext db, int entityId)
    {
        var receta = await db.Recetas.AsNoTracking().SingleOrDefaultAsync(r => r.Id == entityId);
        return receta is null
            ? Error.NotFound(description: "No se encontró la receta.")
            : (receta.Nombre, receta.ImagenUrl);
    }
}
