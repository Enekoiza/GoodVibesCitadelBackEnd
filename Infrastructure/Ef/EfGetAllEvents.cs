namespace Infrastructure.Ef;

using ApplicationService;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class EfGetAllEvents : IGetAllEvents
{
    private readonly AppDbContext db;

    public EfGetAllEvents(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<Event>> Process()
    {
        var cutoff = DateTime.UtcNow.AddMonths(-2);

        return await this.db.Events
            .Where(e => e.EventTime >= cutoff)
            .ToListAsync();
    }
}