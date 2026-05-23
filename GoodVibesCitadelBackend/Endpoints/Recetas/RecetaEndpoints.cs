namespace GoodVibesCitadelBackend.Endpoints.Recetas;

using Infrastructure.Ef;

public static class RecetaEndpoints
{
    public static IEndpointRouteBuilder MapRecetaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/recetas")
            .WithTags("Recetas");

        group.MapGet("/search", Search)
            .RequireAuthorization();

        group.MapGet("/{recetaId:int}/materiales", GetMateriales)
            .RequireAuthorization();

        return app;
    }

    private async static Task<IResult> Search(
        string q,
        int? limit,
        ISearchRecetas searchRecetas)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Results.BadRequest(new { message = "El parámetro 'q' es obligatorio." });

        var results = await searchRecetas.Process(q, limit);
        return Results.Ok(results);
    }

    private async static Task<IResult> GetMateriales(
        int recetaId,
        IGetRecetaMateriales getRecetaMateriales)
    {
        var materiales = await getRecetaMateriales.Process(recetaId);
        return materiales is null
            ? Results.NotFound(new { message = "No se encontró la receta." })
            : Results.Ok(materiales);
    }
}
