namespace ApplicationService;

using Domain.Dto;
using ErrorOr;

public interface IGetBorrowedEventCharacterCredentials
{
    Task<ErrorOr<BorrowedCharacterCredentialsResponseDto>> GetCredentials(
        string eventId,
        string assignedUsername);

    Task<ErrorOr<string>> GetPassword(
        string eventId,
        string assignedUsername,
        string characterName);
}
