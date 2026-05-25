namespace GoodVibesCitadelBackend.Endpoints;

using Auth;
using Events;
using Items;
using PartyBuilder;
using Recetas;
using Roles;
using Users;
using Warehouse;

public static class AllEndpoints
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        app.MapRoleEndpoints();
        app.MapEventEndpoints();
        app.MapPartyBuilderEndpoints();
        app.MapRecetaEndpoints();
        app.MapItemEndpoints();
        app.MapWarehouseEndpoints();

        return app;
    }
}