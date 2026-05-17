namespace GoodVibesCitadelBackend.Endpoints.Users;

using Domain.Entities;
using DTOs;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .RequireAuthorization(x => x.RequireRole("Admin"))
            .WithTags("Users");

        group.MapGet("/getAll", GetAllUserData);
        group.MapPost("/{username}/roles/assignMultiple", AssignMultiple);
        group.MapPost("/{username}/character/updateCharacterList", AssignCharacterInfo);
        group.MapDelete("/{username}/delete", DeleteSingle);

        return app;
    }

    private async static Task<IResult> GetAllUserData(
        UserManager<AppUser> userManager,
        IGetAllCharacters getAllCharacters)
    {
        var allCharacters = getAllCharacters.Process();
        
        var allUsers = await userManager.Users.ToListAsync();
        var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
        var nonAdminUsers = allUsers.Except(adminUsers).ToList();

        var result = new List<AppUserDto>();
        foreach (var user in nonAdminUsers)
        {
            var roles = await userManager.GetRolesAsync(user);

            allCharacters.TryGetValue(user.UserName, out var charactersList);
            
            result.Add(new(user.Id, user.UserName!, user.Email!, roles, charactersList));
        }

        return Results.Ok(result);
    }

    private async static Task<IResult> AssignMultiple(
        [FromBody] UsernameAndRolesDto dto,
        UserManager<AppUser> userManager)
    {
        var user = await userManager.FindByNameAsync(dto.Username);

        if (user == null)
        {
            return Results.NotFound();
        }
        
        var currentRoles = await userManager.GetRolesAsync(user);
        var removeFromAllRolesResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeFromAllRolesResult.Succeeded)
        {
            return Results.BadRequest(removeFromAllRolesResult.Errors);
        }
        
        var addToRolResult = await userManager.AddToRolesAsync(user, dto.Roles);

        if (!addToRolResult.Succeeded)
        {
            return Results.BadRequest(addToRolResult.Errors);
        }
        
        return Results.Ok();
    }

    private async static Task<IResult> AssignCharacterInfo(
        [FromBody] UsernameAndCharacterInfoDto dto,
        UserManager<AppUser> userManager,
        IUpdatedCharacterInformation updatedCharacterInformation)
    {
        var user = await userManager.FindByNameAsync(dto.Username);
        
        if (user == null)
        {
            return Results.BadRequest();
        }

        await updatedCharacterInformation.Process(user.Id, dto.Characters);
        
        return Results.Ok();
    }

    private async static Task<IResult> DeleteSingle(
        string username,
        UserManager<AppUser> userManager)
    {
        var user = await userManager.FindByNameAsync(username);
        
        if (user == null)
        {
            return Results.BadRequest();
        }
        
        await userManager.DeleteAsync(user);
        
        return Results.Ok();
    }
}