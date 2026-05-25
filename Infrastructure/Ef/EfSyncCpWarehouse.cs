namespace Infrastructure.Ef;

using Domain.Dto;
using Domain.Entities;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

public class EfSyncCpWarehouse(AppDbContext db) : ISyncCpWarehouse
{
    public async Task<ErrorOr<List<CpWarehouseEntryDto>>> Process(SyncCpWarehouseDto dto)
    {
        if (dto.Entries is null || dto.Entries.Count == 0)
        {
            return Error.Validation(description: "No hay entradas para sincronizar.");
        }

        var parsedEntries = new List<(int? Id, WarehouseEntryType EntryType, int EntityId, int Quantity)>();

        foreach (var entry in dto.Entries)
        {
            if (!EfAddCpWarehouseEntry.TryParseEntryType(entry.EntryType, out var entryType))
            {
                return Error.Validation(description: "Tipo de entrada no válido. Use Item, Material o Receta.");
            }

            if (entry.EntityId <= 0)
            {
                return Error.Validation(description: "El identificador de la entidad no es válido.");
            }

            if (entry.Quantity <= 0)
            {
                return Error.Validation(description: "La cantidad debe ser un entero mayor que cero.");
            }

            var catalogResult = await CpWarehouseEntryMapper.ResolveCatalogEntry(db, entryType, entry.EntityId);
            if (catalogResult.IsError)
            {
                return catalogResult.Errors;
            }

            parsedEntries.Add((entry.Id, entryType, entry.EntityId, entry.Quantity));
        }

        var duplicateKeys = parsedEntries
            .GroupBy(entry => (entry.EntryType, entry.EntityId))
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateKeys.Count > 0)
        {
            return Error.Validation(description: "Hay entradas duplicadas en la solicitud.");
        }

        var existingEntries = await db.CpWarehouseEntries.ToListAsync();
        var existingById = existingEntries.ToDictionary(entry => entry.Id);
        var existingByKey = existingEntries.ToDictionary(entry => (entry.EntryType, entry.EntityId));

        foreach (var entry in parsedEntries)
        {
            if (entry.Id is int id)
            {
                if (!existingById.TryGetValue(id, out var existing))
                {
                    return Error.NotFound(description: $"Entrada de almacén no encontrada (Id {id}).");
                }

                if (existing.EntryType != entry.EntryType || existing.EntityId != entry.EntityId)
                {
                    return Error.Validation(description: "No se puede cambiar el tipo o la entidad de una entrada existente.");
                }

                existing.Quantity = entry.Quantity;
                existing.UpdatedAt = DateTime.UtcNow;
                continue;
            }

            if (existingByKey.TryGetValue((entry.EntryType, entry.EntityId), out var matched))
            {
                matched.Quantity = entry.Quantity;
                matched.UpdatedAt = DateTime.UtcNow;
                continue;
            }

            var created = new CpWarehouseEntry
            {
                EntryType = entry.EntryType,
                EntityId = entry.EntityId,
                Quantity = entry.Quantity,
                UpdatedAt = DateTime.UtcNow,
            };

            db.CpWarehouseEntries.Add(created);
            existingByKey[(entry.EntryType, entry.EntityId)] = created;
        }

        await db.SaveChangesAsync();

        return await new EfGetCpWarehouse(db).Process();
    }
}
