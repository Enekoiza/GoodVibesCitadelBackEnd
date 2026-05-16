namespace GoodVibesCitadelBackend.Endpoints.Users.DTOs;

public record EventDto(string Username, DateTime EventTime, string EventName, string EventType);