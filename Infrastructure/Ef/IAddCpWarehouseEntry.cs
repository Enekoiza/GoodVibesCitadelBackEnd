namespace Infrastructure.Ef;

using Domain.Dto;
using ErrorOr;

public interface IAddCpWarehouseEntry
{
    Task<ErrorOr<CpWarehouseEntryDto>> Process(AddCpWarehouseEntryDto dto);
}
