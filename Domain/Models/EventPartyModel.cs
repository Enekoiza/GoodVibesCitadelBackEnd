namespace Domain.Models;

public record EventPartyModel(EventModel Event, List<SlotModel> Slots, string OwnerUserId, bool ReplaceExisting = false);