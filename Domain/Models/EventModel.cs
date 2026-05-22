namespace Domain.Models;

public record EventModel(string EventId, string UserId, DateTime EventTime, string EventName, string EventType);