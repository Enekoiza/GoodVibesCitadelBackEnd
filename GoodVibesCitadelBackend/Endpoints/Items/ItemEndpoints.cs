namespace GoodVibesCitadelBackend.Endpoints.Items;

using Domain.Dto;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Mvc;

public static class ItemEndpoints
{
    public static IEndpointRouteBuilder MapItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/items")
            .WithTags("Items");

        group.MapPost("/lookup", Lookup)
            .RequireAuthorization();

        return app;
    }

    private async static Task<IResult> Lookup(
        [FromBody] ItemLookupRequestDto dto,
        ILookupItemsByNames lookupItemsByNames)
    {
        if (dto.Names is null || dto.Names.Count == 0)
        {
            return Results.Ok(Array.Empty<ItemLookupDto>());
        }

        var results = await lookupItemsByNames.Process(dto.Names);
        return Results.Ok(results);
    }
}
