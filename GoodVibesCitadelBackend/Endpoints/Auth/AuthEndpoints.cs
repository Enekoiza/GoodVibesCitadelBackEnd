namespace GoodVibesCitadelBackend.Endpoints.Auth;

using Domain;
using Domain.Entities;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        group.MapPost("/login", Login);
        group.MapPost("/register", Register);
        group.MapPost("/verifyTemporaryPassword", TemporaryPasswordVerification);
        group.MapPost("/updatePassword", UpdatePassword);

        return app;
    }

    private async static Task<IResult> Login(
        LoginRequest request,
        UserManager<AppUser> userManager,
        IConfiguration config)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Results.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!)
        };

        foreach (var role in roles)
            claims.Add(new(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["goodvibescitadeljwtkey"]!))
        {
            KeyId = "goodvibes-jwt-1"
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return Results.Ok(new { accessToken = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    private async static Task<IResult> Register(
        OnlyUsernameRequest request,
        UserManager<AppUser> userManager)
    {
        var randomPassword = RandomPasswordGenerator.Generate();

        var user = new AppUser { UserName = request.Username, IsPasswordTemporary = true};
        var result = await userManager.CreateAsync(user, randomPassword);

        return result.Succeeded
            ? Results.Ok(new { password = randomPassword })
            : Results.BadRequest(result.Errors);
    }

    private async static Task<IResult> TemporaryPasswordVerification(
        UsernameOnlyRequest request,
        UserManager<AppUser> userManager)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        
        if (user == null) return Results.NotFound();

        if (user.IsPasswordTemporary)
        {
            return Results.BadRequest();
        }

        return Results.Ok();
    }

    private async static Task<IResult> UpdatePassword(
        UpdatePasswordRequest request,
        UserManager<AppUser> userManager,
        IUpdatePasswordTemporaryFlag updatePasswordTemporaryFlag)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        
        if (user == null) return Results.NotFound();
        
        var updateResult = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!updateResult.Succeeded)
        {
            return Results.BadRequest(updateResult.Errors);
        }

        await updatePasswordTemporaryFlag.Process(user.Id);

        return Results.Ok();
    }
}