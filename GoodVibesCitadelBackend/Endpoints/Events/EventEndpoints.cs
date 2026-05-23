namespace GoodVibesCitadelBackend.Endpoints.Events;

using ApplicationService;
using Domain.Dto;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public static class EventEndpoints
{
    public static IEndpointRouteBuilder MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event")
            .WithTags("Users");

        group.MapPost("/create", Create);
        group.MapGet("/getAll", GetAll);
        group.MapPost("/attachparty", AttachParty);
        group.MapPost("/updatedrops", UpdateDrops);

        return app;
    }

    private async static Task<IResult> Create(
        [FromBody] EventDto dto,
        UserManager<AppUser> userManager,
        IAddNewEvent addNewEvent)
    {
        var user = await userManager.FindByNameAsync(dto.Username);
        if (user == null)
        {
            return Results.BadRequest();
        }
        
        await addNewEvent.Process(MapIntoEventModel(user.Id, dto));
        
        return Results.Ok();
    }

    private async static Task<IResult> GetAll(
        IGetAllEventsProcessor getAllEvents,
        UserManager<AppUser> userManager)
    {
        var allEvents = await getAllEvents.Process();
        
        return Results.Ok(allEvents);
    }

    private async static Task<IResult> AttachParty(
        [FromBody] EventPartyDto dto,
        UserManager<AppUser> userManager,
        IAddNewEventPartyCombination addNewEventPartyCombination)
    {
        var user = await userManager.FindByNameAsync(dto.AssignedByUsername);
        if (user == null)
        {
            return Results.BadRequest();
        }
        
        await addNewEventPartyCombination.Process(MapIntoEventPartyModel(user.Id, dto));
        
        return Results.Ok();
    }

    private async static Task<IResult> UpdateDrops(
        [FromBody] EventDropsDto dto,
        IUpdateEventDrops updateEventDrops)
    {
        if (string.IsNullOrWhiteSpace(dto.EventId))
        {
            return Results.BadRequest("EventId es obligatorio.");
        }

        if (dto.Drops.Any(drop => string.IsNullOrWhiteSpace(drop.Name) || drop.Quantity <= 0))
        {
            return Results.BadRequest("Cada material debe tener nombre y cantidad mayor que cero.");
        }

        var result = await updateEventDrops.Process(MapIntoEventDropsModel(dto));
        return result.IsError ? Results.NotFound(result.FirstError.Description) : Results.Ok();
    }

    private static EventModel MapIntoEventModel(string userId, EventDto eventDto)
    {
        return new(eventDto.EventId, userId, eventDto.EventTime, eventDto.EventName, eventDto.EventType);
    }

    private static EventPartyModel MapIntoEventPartyModel(string userId, EventPartyDto eventPartyDto)
    {
        return new(
            OwnerUserId: userId,
            Event: new(
                eventPartyDto.Event.EventId,
                string.Empty,
                eventPartyDto.Event.EventTime,
                eventPartyDto.Event.EventName,
                eventPartyDto.Event.EventType),
            Slots: eventPartyDto.Slots.Select(MapIntoSlotModel).ToList(),
            ReplaceExisting: eventPartyDto.ReplaceExisting);
    }

    private static SlotModel MapIntoSlotModel(SlotDto slotDto)
    {
        return new(slotDto.Role, slotDto.UserId, slotDto.Username, slotDto.CharacterName);
    }

    private static EventDropsModel MapIntoEventDropsModel(EventDropsDto dto)
    {
        return new(
            dto.EventId,
            dto.Drops.Select(drop => new EventDropModel(drop.Name, drop.Quantity)).ToList());
    }
}