namespace Domain.Dto;

public record BorrowedCharacterCredentialsDto(
    string CharacterName,
    string OwnerUsername,
    string Login,
    bool HasPassword);

public record BorrowedCharacterCredentialsResponseDto(
    string Visibility,
    IReadOnlyList<BorrowedCharacterCredentialsDto> Characters);

public static class BorrowedCharacterCredentialsVisibility
{
    public const string None = "none";
    public const string Scheduled = "scheduled";
    public const string Available = "available";
    public const string Expired = "expired";
}
