namespace Domain.Models;

public record EventDropModel(string Name, decimal Quantity);

public record EventDropsModel(string EventId, List<EventDropModel> Drops);
