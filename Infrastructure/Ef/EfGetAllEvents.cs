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
        var eventList = await this.db.Events.ToListAsync();
        
        return eventList;
    }
}