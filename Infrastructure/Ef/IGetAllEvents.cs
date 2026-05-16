namespace Infrastructure.Ef;

using Domain.Entities;

public interface IGetAllEvents
{
    Task<List<Event>> Process();
}