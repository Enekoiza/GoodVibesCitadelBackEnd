namespace Infrastructure.Ef;

using Domain.Dto;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

public class EfUpdateCpWarehouseEntry(AppDbContext db) : IUpdateCpWarehouseEntry
{
    public async Task<ErrorOr<CpWarehouseEntryDto>> Process(int id, UpdateCpWarehouseEntryDto dto)
    {
        if (dto.Quantity <= 0)
        {
            return Error.Validation(description: "La cantidad debe ser mayor que cero.");
        }

        var entry = await db.CpWarehouseEntries.SingleOrDefaultAsync(e => e.Id == id);
        if (entry is null)
        {
            return Error.NotFound(description: "Entrada de almacén no encontrada.");
        }

        entry.Quantity = dto.Quantity;
        entry.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var mapped = await CpWarehouseEntryMapper.MapEntries(db, [entry]);
        return mapped[0];
    }
}
