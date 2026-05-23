namespace ApplicationService;

using Domain.Models;
using ErrorOr;

public interface IUpdateEventDrops
{
    Task<ErrorOr<Success>> Process(EventDropsModel eventDropsModel);
}
