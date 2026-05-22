namespace Domain.Entities;

public record PartyCompositionEntity(string Owner, List<SlotEntity> Slots);