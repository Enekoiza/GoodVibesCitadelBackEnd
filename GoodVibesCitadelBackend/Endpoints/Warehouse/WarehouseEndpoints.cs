namespace GoodVibesCitadelBackend.Endpoints.Warehouse;

using Domain.Dto;
using ErrorOr;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Mvc;

public static class WarehouseEndpoints
{
    private static readonly string[] WarehouseEditRoles = ["Admin", "CP Admin", "CP Oficial"];

    public static IEndpointRouteBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/warehouse")
            .WithTags("Warehouse");

        group.MapGet("/", GetAll)
            .RequireAuthorization();

        group.MapGet("/catalog-search", CatalogSearch)
            .RequireAuthorization(x => x.RequireRole(WarehouseEditRoles));

        group.MapPost("/", AddEntry)
            .RequireAuthorization(x => x.RequireRole(WarehouseEditRoles));

        group.MapPut("/sync", SyncEntries)
            .RequireAuthorization(x => x.RequireRole(WarehouseEditRoles));

        group.MapPut("/{id:int}", UpdateEntry)
            .RequireAuthorization(x => x.RequireRole(WarehouseEditRoles));

        group.MapDelete("/{id:int}", DeleteEntry)
            .RequireAuthorization(x => x.RequireRole(WarehouseEditRoles));

        return app;
    }

    private async static Task<IResult> GetAll(IGetCpWarehouse getCpWarehouse)
    {
        var entries = await getCpWarehouse.Process();
        return Results.Ok(entries);
    }

    private async static Task<IResult> CatalogSearch(
        string q,
        string? type,
        int? limit,
        ISearchWarehouseCatalog searchWarehouseCatalog)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Results.BadRequest(new { message = "El parámetro 'q' es obligatorio." });
        }

        var results = string.IsNullOrWhiteSpace(type)
            ? await searchWarehouseCatalog.ProcessAll(q, limit)
            : await searchWarehouseCatalog.Process(type, q, limit);

        return Results.Ok(results);
    }

    private async static Task<IResult> SyncEntries(
        [FromBody] SyncCpWarehouseDto dto,
        ISyncCpWarehouse syncCpWarehouse)
    {
        var result = await syncCpWarehouse.Process(dto);
        return MapListResult(result);
    }

    private async static Task<IResult> AddEntry(
        [FromBody] AddCpWarehouseEntryDto dto,
        IAddCpWarehouseEntry addCpWarehouseEntry)
    {
        var result = await addCpWarehouseEntry.Process(dto);
        return MapEntryResult(result, successStatusCode: StatusCodes.Status201Created);
    }

    private async static Task<IResult> UpdateEntry(
        int id,
        [FromBody] UpdateCpWarehouseEntryDto dto,
        IUpdateCpWarehouseEntry updateCpWarehouseEntry)
    {
        var result = await updateCpWarehouseEntry.Process(id, dto);
        return MapEntryResult(result);
    }

    private async static Task<IResult> DeleteEntry(
        int id,
        IDeleteCpWarehouseEntry deleteCpWarehouseEntry)
    {
        var result = await deleteCpWarehouseEntry.Process(id);
        return MapSuccessResult(result);
    }

    private static IResult MapListResult(ErrorOr<List<CpWarehouseEntryDto>> result)
    {
        if (result.IsError)
        {
            return MapError(result.FirstError);
        }

        return Results.Ok(result.Value);
    }

    private static IResult MapEntryResult(ErrorOr<CpWarehouseEntryDto> result, int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsError)
        {
            return MapError(result.FirstError);
        }

        return Results.Json(result.Value, statusCode: successStatusCode);
    }

    private static IResult MapSuccessResult(ErrorOr<Success> result)
    {
        if (result.IsError)
        {
            return MapError(result.FirstError);
        }

        return Results.NoContent();
    }

    private static IResult MapError(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new { message = error.Description }),
            ErrorType.Validation => Results.BadRequest(new { message = error.Description }),
            _ => Results.Problem(error.Description),
        };
}
