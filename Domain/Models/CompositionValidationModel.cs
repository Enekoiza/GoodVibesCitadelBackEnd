namespace Domain.Models;

public record CompositionValidationModel(
    PartyType PartyType,
    int DpsCount,
    int BishopCount,
    int BardCount,
    int BufferCount,
    int TankCount,
    int RechargerCount,
    bool IsPartyFull);