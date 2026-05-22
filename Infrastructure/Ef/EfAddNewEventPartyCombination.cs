namespace Infrastructure.Ef;

using ApplicationService;
using Domain.Entities;
using Domain.Models;
using ErrorOr;

public class EfAddNewEventPartyCombination : IAddNewEventPartyCombination
{
    private readonly AppDbContext db;

    public EfAddNewEventPartyCombination(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<ErrorOr<Success>> Process(EventPartyModel eventPartyModel)
    {
        var eventObject = this.db.Events.Single(x => x.Id.ToString() == eventPartyModel.Event.EventId);

        var partyCompositionList = new PartyCompositionEntity(eventPartyModel.OwnerUserId, eventPartyModel.Slots.Select(MapSlots).ToList());

        if (eventPartyModel.ReplaceExisting || eventObject.PartyComposition is null)
        {
            eventObject.PartyComposition = [partyCompositionList];
        }
        else
        {
            eventObject.PartyComposition = [..eventObject.PartyComposition, partyCompositionList];
        }

        await this.db.SaveChangesAsync();

        return Result.Success;
    }

    private SlotEntity MapSlots(SlotModel slotModel)
    {
        return new(slotModel.Role, slotModel.Username, slotModel.CharacterName);
    }
}