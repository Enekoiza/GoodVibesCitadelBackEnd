namespace Shared.DTO;

/// <summary>
/// Character data accepted from clients. <see cref="Password"/> is plaintext when provided:
/// null = keep existing hash on update; empty string = clear password; non-empty = set new password (hashed server-side).
/// </summary>
public record CharacterInfoUpdate(
    string Name,
    string ClassName,
    string ClassType,
    int Level,
    string Login,
    string? Password);
