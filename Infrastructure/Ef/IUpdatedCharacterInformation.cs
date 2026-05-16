namespace Infrastructure.Ef;

using ErrorOr;
using Shared.DTO;

public interface IUpdatedCharacterInformation
{
    Task<ErrorOr<Success>> Process(string userId, List<CharacterInfo> characterInfos);
}