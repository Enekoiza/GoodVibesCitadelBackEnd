namespace Domain.Models;

public record CompositionModel(
    string Name, 
    int DpsCount, 
    int BishopCount, 
    int BardCount, 
    int BufferCount, 
    int TankCount, 
    int RechargerCount, 
    bool IsPartyFull);