namespace Domain.Dto;

public record CompositionValidationDto(
    string PartyType,
    int DpsCount,
    int BishopCount,
    int BardCount,
    int BufferCount,
    int TankCount,
    int RechargerCount,
    bool IsPartyFull);