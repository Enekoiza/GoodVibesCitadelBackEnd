namespace Domain.Dto;

public record PartyCompositionResponseDto(string Owner, IEnumerable<SlotDto> Slots);