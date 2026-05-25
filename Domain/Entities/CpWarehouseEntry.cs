namespace Domain.Entities;

public class CpWarehouseEntry
{
    public int Id { get; set; }

    public WarehouseEntryType EntryType { get; set; }

    public int EntityId { get; set; }

    public int Quantity { get; set; }

    public DateTime UpdatedAt { get; set; }
}
