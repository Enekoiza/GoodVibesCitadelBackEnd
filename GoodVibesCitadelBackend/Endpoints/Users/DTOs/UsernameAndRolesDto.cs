namespace GoodVibesCitadelBackend.Endpoints.Users.DTOs;

public record UsernameAndRolesDto(string Username, List<string> Roles);