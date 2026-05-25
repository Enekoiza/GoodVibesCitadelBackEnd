namespace Infrastructure.Ef;

using Domain.Dto;
using ErrorOr;

public interface IUpdateCpWarehouseEntry
{
    Task<ErrorOr<CpWarehouseEntryDto>> Process(int id, UpdateCpWarehouseEntryDto dto);
}
