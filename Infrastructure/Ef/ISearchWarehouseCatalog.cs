namespace Infrastructure.Ef;

using Domain.Dto;

public interface ISearchWarehouseCatalog
{
    Task<List<WarehouseCatalogResultDto>> Process(string entryType, string query, int? limit = 20);

    Task<List<WarehouseCatalogResultDto>> ProcessAll(string query, int? limit = 20);
}
