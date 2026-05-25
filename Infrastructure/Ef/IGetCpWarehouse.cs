namespace Infrastructure.Ef;

using Domain.Dto;

public interface IGetCpWarehouse
{
    Task<List<CpWarehouseEntryDto>> Process();
}
