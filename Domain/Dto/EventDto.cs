namespace Domain.Dto;

public record EventDto(
    string EventId,
    string Username,
    DateTime EventTime,
    string EventName,
    string EventType,
    IEnumerable<PartyCompositionResponseDto> PartyCompositionResponseDtos,
    IEnumerable<EventDropDto> Drops);