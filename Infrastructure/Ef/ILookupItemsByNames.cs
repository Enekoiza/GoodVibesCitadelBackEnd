namespace Infrastructure.Ef;

using Domain.Dto;

public interface ILookupItemsByNames
{
    Task<List<ItemLookupDto>> Process(IReadOnlyList<string> names);
}
