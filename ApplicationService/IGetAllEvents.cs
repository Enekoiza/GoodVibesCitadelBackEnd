namespace ApplicationService;

using Domain.Entities;

public interface IGetAllEvents
{
    Task<List<Event>> Process();
}