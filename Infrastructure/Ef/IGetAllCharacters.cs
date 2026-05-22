namespace Infrastructure.Ef;

using Shared.DTO;

public interface IGetAllCharacters
{
    IReadOnlyDictionary<string, List<CharacterInfoResponse>> Process();
}