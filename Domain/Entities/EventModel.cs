namespace Domain.Entities;

public record EventModel(string UserId, DateTime EventTime, string EventName, string EventType);