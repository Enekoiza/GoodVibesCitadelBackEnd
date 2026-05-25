namespace Infrastructure.Ef;

using Domain.Dto;
using ErrorOr;

public interface ISyncCpWarehouse
{
    Task<ErrorOr<List<CpWarehouseEntryDto>>> Process(SyncCpWarehouseDto dto);
}
