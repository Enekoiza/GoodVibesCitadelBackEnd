namespace GoodVibesCitadelBackend.Endpoints.Events;

using Domain.Dto;
using Domain.Entities;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Users.DTOs;

public static class EventEndpoints
{
    public static IEndpointRouteBuilder MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event")
            .WithTags("Users");

        group.MapPost("/create", Create);
        group.MapGet("/getAll", GetAll);

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
        IGetAllEvents getAllEvents,
        UserManager<AppUser> userManager)
    {
        var allEvents = await getAllEvents.Process();
        
        var finalResult = new List<EventResponse>();

        foreach (var event1 in allEvents)
        {
            var user = await userManager.FindByIdAsync(event1.UserId);
            
            var username = await userManager.GetUserNameAsync(user);
            
            finalResult.Add(new(username, event1.EventTime, event1.Name, event1.EventType));
        }
        
        return Results.Ok(finalResult);
    }

    private static EventModel MapIntoEventModel(string userId, EventDto eventDto)
    {
        return new(userId, eventDto.EventTime, eventDto.EventName, eventDto.EventType);
    }
}