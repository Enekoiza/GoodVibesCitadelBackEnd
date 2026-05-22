namespace ApplicationService;

using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class GetAllEventsProcessor(IGetAllEvents getAllEvents, UserManager<AppUser> userManager) : IGetAllEventsProcessor
{
    public async Task<List<EventDto>> Process()
    {
        var events = await getAllEvents.Process();

        var eventDtos = new List<EventDto>();

        foreach (var evento in events)  
        {
            var userName = await userManager.FindByIdAsync(evento.UserId);
            
            if (evento.PartyComposition is null)
            {
                
                eventDtos.Add(
                    new(evento.Id.ToString(),
                        userName.UserName,
                        evento.EventTime,
                        evento.Name,
                        evento.EventType,
                        [])
                );
            }
            else
            {
                eventDtos.Add(
                    new(evento.Id.ToString(),
                        userName.UserName,
                        evento.EventTime,
                        evento.Name,
                        evento.EventType,
                        evento.PartyComposition.Select(MapCompositionToDto)));
            }
        }
        
        return eventDtos;
    }

    private static PartyCompositionResponseDto MapCompositionToDto(PartyCompositionEntity partyCompositionEntity)
    {
        return new(
            partyCompositionEntity.Owner,
            partyCompositionEntity.Slots.Select(MapSlotToDto));
    }

    private static SlotDto MapSlotToDto(SlotEntity slotEntity)
    {
        return new(slotEntity.Role, string.Empty, slotEntity.Username, slotEntity.CharacterName);
    }
}