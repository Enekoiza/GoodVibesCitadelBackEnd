namespace Shared.DTO;

/// <summary>
/// Updates login/password for one character. Password omitted or null = keep stored value;
/// empty string = clear; non-empty = new password (encrypted server-side).
/// </summary>
public record UpdateCharacterCredentialsDto(string Login, string? Password);
