namespace GoodVibesCitadelBackend.Endpoints.Users.DTOs;

using Shared.DTO;

public record AppUserDto(
    string Id,
    string UserName,
    string Email,
    IList<string> Roles,
    IList<CharacterInfo> Characters
);