namespace GoodVibesCitadelBackend.Endpoints.PartyBuilder;

using ApplicationService;
using Domain;
using Domain.Dto;
using Domain.Models;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Mvc;

public static class PartyBuilderEndpoints
{
    public static IEndpointRouteBuilder MapPartyBuilderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/builder")
            .WithTags("Builder");

        group.MapGet("/getComps", GetCompositions);
        group.MapPost("/validate", ValidateComposition);

        return app;
    }

    private async static Task<IResult> GetCompositions(
        IGetAllPartyCompositions getAllPartyCompositions)
    {
        var allCompositions = await getAllPartyCompositions.Process();

        var compositionDtos = allCompositions.Select(MapToCompositionDto);

        return Results.Ok(compositionDtos);
    }

    private static IResult ValidateComposition(
        [FromBody] CompositionValidationDto dto,
        ICompositionValidation compositionValidation)
    {
        var compositionValidationModel = MapToCompositionValidationModel(dto);

        var compositionValidationResult = compositionValidation.Validate(compositionValidationModel);

        return Results.Ok(compositionValidationResult);
    }

    private static CompositionDto MapToCompositionDto(CompositionModel composition)
    {
        return new(composition.Name,
            composition.DpsCount,
            composition.BishopCount,
            composition.BardCount,
            composition.BufferCount,
            composition.TankCount,
            composition.RechargerCount,
            composition.IsPartyFull);
    }

    private static CompositionValidationModel MapToCompositionValidationModel(CompositionValidationDto dto)
    {
        Enum.TryParse(dto.PartyType, out PartyType partyTypeEnum);
        
        return new(partyTypeEnum,
            dto.DpsCount,
            dto.BishopCount,
            dto.BardCount,
            dto.BufferCount,
            dto.TankCount,
            dto.RechargerCount,
            dto.IsPartyFull);
    }
}