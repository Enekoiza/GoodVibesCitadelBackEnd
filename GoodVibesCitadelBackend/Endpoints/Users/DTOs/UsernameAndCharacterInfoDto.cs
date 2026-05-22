namespace GoodVibesCitadelBackend.Endpoints.Users.DTOs;

using Shared.DTO;

public record UsernameAndCharacterInfoDto(string Username, List<CharacterInfoUpdate> Characters);