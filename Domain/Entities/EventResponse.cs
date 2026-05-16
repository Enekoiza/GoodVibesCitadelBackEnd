namespace Domain.Entities;

public record EventResponse(string username, DateTime eventTime, string eventName, string eventType);