namespace Infrastructure.Ef;

using Domain.Dto;

public interface ISearchRecetas
{
    Task<List<RecetaDto>> Process(string query, int? limit = 20);
}
