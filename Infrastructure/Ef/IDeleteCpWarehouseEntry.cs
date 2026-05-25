namespace Infrastructure.Ef;

using ErrorOr;

public interface IDeleteCpWarehouseEntry
{
    Task<ErrorOr<Success>> Process(int id);
}
