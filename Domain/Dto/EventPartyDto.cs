namespace Domain.Dto;

public record EventPartyDto(EventDto Event, List<SlotDto> Slots, string AssignedByUsername, bool ReplaceExisting = false);