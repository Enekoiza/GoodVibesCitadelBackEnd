namespace Domain.Dto;

public record EventResponse(string Username, DateTime EventTime, string EventName, string EventType);