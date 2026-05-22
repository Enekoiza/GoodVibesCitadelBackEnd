namespace GoodVibesCitadelBackend.Endpoints.Users;

using Domain.Entities;
using DTOs;
using Infrastructure;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Shared.DTO;
using System.Security.Claims;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        group.MapGet("/getAll", GetAllUserData)
            .RequireAuthorization();

        group.MapGet("/me/characters", GetMyCharacters)
            .RequireAuthorization();
        group.MapPost("/me/characters", UpdateMyCharacters)
            .RequireAuthorization();
        group.MapGet("/me/characters/{characterName}/password", GetMyCharacterPassword)
            .RequireAuthorization();
        group.MapPut("/me/characters/{characterName}/credentials", UpdateMyCharacterCredentials)
            .RequireAuthorization();

        group.MapPost("/{username}/roles/assignMultiple", AssignMultiple)
            .RequireAuthorization(x => x.RequireRole("Admin", "CP Admin"));
        group.MapPost("/{username}/character/updateCharacterList", AssignCharacterInfo)
            .RequireAuthorization(x => x.RequireRole("Admin", "CP Admin"));
        group.MapDelete("/{username}/delete", DeleteSingle)
            .RequireAuthorization(x => x.RequireRole("Admin", "CP Admin"));

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

    private static Task<IResult> GetMyCharacters(
        ClaimsPrincipal principal,
        IGetAllCharacters getAllCharacters)
    {
        var username = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
        {
            return Task.FromResult(Results.Unauthorized());
        }

        var allCharacters = getAllCharacters.Process();
        if (!allCharacters.TryGetValue(username, out var characters) || characters is null)
        {
            return Task.FromResult(Results.Ok(Array.Empty<CharacterInfoResponse>()));
        }

        return Task.FromResult(Results.Ok(characters));
    }

    private async static Task<IResult> UpdateMyCharacters(
        [FromBody] List<CharacterInfoUpdate> characters,
        ClaimsPrincipal principal,
        UserManager<AppUser> userManager,
        IUpdatedCharacterInformation updatedCharacterInformation)
    {
        var username = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
        {
            return Results.Unauthorized();
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            return Results.NotFound();
        }

        await updatedCharacterInformation.Process(user.Id, characters);
        return Results.Ok();
    }

    private async static Task<IResult> GetMyCharacterPassword(
        string characterName,
        ClaimsPrincipal principal,
        UserManager<AppUser> userManager,
        AppDbContext dbContext,
        ICharacterPasswordProtector passwordProtector)
    {
        var username = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
        {
            return Results.Unauthorized();
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            return Results.NotFound();
        }

        var character = await dbContext.Characters
            .SingleOrDefaultAsync(c => c.Userid == user.Id && c.Name == characterName);

        if (character is null)
        {
            return Results.NotFound();
        }

        if (string.IsNullOrEmpty(character.Password))
        {
            return Results.Ok(new CharacterPasswordRevealDto(string.Empty));
        }

        if (!passwordProtector.TryReveal(character.Password, out var plainPassword))
        {
            return Results.Conflict(new
            {
                message = "Escribe la contraseña de nuevo en editar credenciales y guarda para actualizarla.",
            });
        }

        return Results.Ok(new CharacterPasswordRevealDto(plainPassword));
    }

    private async static Task<IResult> UpdateMyCharacterCredentials(
        string characterName,
        [FromBody] UpdateCharacterCredentialsDto dto,
        ClaimsPrincipal principal,
        UserManager<AppUser> userManager,
        IUpdatedCharacterInformation updatedCharacterInformation)
    {
        var username = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
        {
            return Results.Unauthorized();
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            return Results.NotFound();
        }

        var result = await updatedCharacterInformation.UpdateCredentials(
            user.Id,
            characterName,
            dto.Login,
            dto.Password);

        return result.Match(
            _ => Results.Ok(),
            errors => errors.First().Type == ErrorType.Validation
                ? Results.BadRequest(new { message = errors.First().Description })
                : Results.Problem(errors.First().Description));
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