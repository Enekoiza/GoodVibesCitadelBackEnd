namespace Domain.Entities;

public class Event
{
    public int Id { get; set; }

    public string EventType { get; set; } = null!;

    public DateTime EventTime { get; set; }

    public DateTime Ts { get; set; }

    public string UserId { get; set; } = null!;

    public string Name { get; set; }

    public virtual AppUser User { get; set; } = null!;
    
    public List<PartyCompositionEntity>? PartyComposition { get; set; }

    public List<EventDropEntity>? Drops { get; set; }
}