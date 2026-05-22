namespace Infrastructure.Ef;

using ErrorOr;
using Shared.DTO;

public interface IUpdatedCharacterInformation
{
    Task<ErrorOr<Success>> Process(string userId, List<CharacterInfoUpdate> characterInfos);

    Task<ErrorOr<Success>> UpdateCredentials(
        string userId,
        string characterName,
        string login,
        string? password);
}