namespace Infrastructure.Ef;

using Domain.Dto;
using Domain.Entities;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

public class EfAddCpWarehouseEntry(AppDbContext db) : IAddCpWarehouseEntry
{
    public async Task<ErrorOr<CpWarehouseEntryDto>> Process(AddCpWarehouseEntryDto dto)
    {
        if (!TryParseEntryType(dto.EntryType, out var entryType))
        {
            return Error.Validation(description: "Tipo de entrada no válido. Use Item, Material o Receta.");
        }

        if (dto.EntityId <= 0)
        {
            return Error.Validation(description: "El identificador de la entidad no es válido.");
        }

        if (dto.Quantity <= 0)
        {
            return Error.Validation(description: "La cantidad debe ser mayor que cero.");
        }

        var catalogResult = await CpWarehouseEntryMapper.ResolveCatalogEntry(db, entryType, dto.EntityId);
        if (catalogResult.IsError)
        {
            return catalogResult.Errors;
        }

        var existing = await db.CpWarehouseEntries
            .SingleOrDefaultAsync(e => e.EntryType == entryType && e.EntityId == dto.EntityId);

        if (existing is not null)
        {
            existing.Quantity += dto.Quantity;
            existing.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            var (nombre, imagenUrl) = catalogResult.Value;
            return new CpWarehouseEntryDto(
                existing.Id,
                entryType.ToString(),
                existing.EntityId,
                nombre,
                imagenUrl,
                existing.Quantity);
        }

        var entry = new CpWarehouseEntry
        {
            EntryType = entryType,
            EntityId = dto.EntityId,
            Quantity = dto.Quantity,
            UpdatedAt = DateTime.UtcNow,
        };

        db.CpWarehouseEntries.Add(entry);
        await db.SaveChangesAsync();

        var catalog = catalogResult.Value;
        return new CpWarehouseEntryDto(
            entry.Id,
            entryType.ToString(),
            entry.EntityId,
            catalog.Nombre,
            catalog.ImagenUrl,
            entry.Quantity);
    }

    internal static bool TryParseEntryType(string? value, out WarehouseEntryType entryType)
    {
        entryType = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return Enum.TryParse(value.Trim(), ignoreCase: true, out entryType)
               && Enum.IsDefined(entryType);
    }
}
