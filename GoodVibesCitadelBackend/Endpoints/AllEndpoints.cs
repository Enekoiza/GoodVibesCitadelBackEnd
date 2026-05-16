namespace GoodVibesCitadelBackend.Endpoints;

using Auth;
using Events;
using Roles;
using Users;

public static class AllEndpoints
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        app.MapRoleEndpoints();
        app.MapEventEndpoints();

        return app;
    }
}