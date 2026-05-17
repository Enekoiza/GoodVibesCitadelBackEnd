namespace Domain.Entities;

public class Composition
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public int DpsCount { get; set; }

    public int BufferCount { get; set; }

    public int BardCount { get; set; }

    public int BishopCount { get; set; }

    public int RechargerCount { get; set; }

    public int TankCount { get; set; }

    public bool IsPartyFull { get; set; }
}