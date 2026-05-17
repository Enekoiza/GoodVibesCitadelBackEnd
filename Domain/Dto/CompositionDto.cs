namespace Domain.Dto;

public record CompositionDto(
    string Name, 
    int DpsCount, 
    int BishopCount, 
    int BardCount, 
    int BufferCount, 
    int TankCount, 
    int RechargerCount, 
    bool IsPartyFull);