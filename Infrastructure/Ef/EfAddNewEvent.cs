namespace Infrastructure.Ef;

using Domain.Models;

public class EfAddNewEvent : IAddNewEvent
{
    private readonly AppDbContext db;

    public EfAddNewEvent(AppDbContext db)
    {
        this.db = db;
    }

    public async Task Process(EventModel eventData)
    {
        this.db.Events.Add(new()
        {
            EventType = eventData.EventType,
            EventTime = eventData.EventTime,
            Ts = DateTime.UtcNow,
            UserId = eventData.UserId,
            Name = eventData.EventName
        });

        await this.db.SaveChangesAsync();
    }
}