namespace Infrastructure.Ef;

using ApplicationService;
using Domain.Entities;
using Domain.Models;
using ErrorOr;

public class EfUpdateEventDrops : IUpdateEventDrops
{
    private readonly AppDbContext db;

    public EfUpdateEventDrops(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<ErrorOr<Success>> Process(EventDropsModel eventDropsModel)
    {
        var eventObject = this.db.Events.SingleOrDefault(x => x.Id.ToString() == eventDropsModel.EventId);
        if (eventObject is null)
        {
            return Error.NotFound(description: "Evento no encontrado.");
        }

        eventObject.Drops = eventDropsModel.Drops
            .Select(drop => new EventDropEntity(drop.Name, drop.Quantity))
            .ToList();

        await this.db.SaveChangesAsync();

        return Result.Success;
    }
}
