namespace Shared.DTO;

/// <summary>Character data returned to clients. Never includes the password.</summary>
public record CharacterInfoResponse(
    string Name,
    string ClassName,
    string ClassType,
    int Level,
    string Login,
    bool HasPassword);
