namespace Infrastructure.Ef;

using ErrorOr;
using Microsoft.EntityFrameworkCore;

public class EfDeleteCpWarehouseEntry(AppDbContext db) : IDeleteCpWarehouseEntry
{
    public async Task<ErrorOr<Success>> Process(int id)
    {
        var entry = await db.CpWarehouseEntries.SingleOrDefaultAsync(e => e.Id == id);
        if (entry is null)
        {
            return Error.NotFound(description: "Entrada de almacén no encontrada.");
        }

        db.CpWarehouseEntries.Remove(entry);
        await db.SaveChangesAsync();

        return Result.Success;
    }
}
