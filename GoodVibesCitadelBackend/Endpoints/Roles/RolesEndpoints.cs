namespace GoodVibesCitadelBackend.Endpoints.Roles;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class RolesEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles")
            .RequireAuthorization(x => x.RequireRole("Admin"))
            .WithTags("Roles");

        group.MapGet("/getAll", GetAllRoles);
        group.MapPost("/create/{roleName}", Create);
        group.MapDelete("/delete/{roleName}", Delete);

        return app;
    }
    
    private async static Task<IResult> GetAllRoles(
        RoleManager<IdentityRole> roleManager)
    {
        var allRoles = await roleManager.Roles.ToListAsync();
        
        var allRolesNames = allRoles.Select(x => x.Name);
        
        return Results.Ok(allRolesNames);
    }
    
    private async static Task<IResult> Create(
        string roleName,
        RoleManager<IdentityRole> roleManager)
    {
        var identityResult = await roleManager.CreateAsync(new (roleName));

        return identityResult.Succeeded
            ? Results.Ok()
            : Results.BadRequest(identityResult.Errors);
    }
    
    private async static Task<IResult> Delete(
        string roleName,
        RoleManager<IdentityRole> roleManager)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null) return Results.NotFound();

        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded
            ? Results.Ok()
            : Results.BadRequest(result.Errors);
    }
}