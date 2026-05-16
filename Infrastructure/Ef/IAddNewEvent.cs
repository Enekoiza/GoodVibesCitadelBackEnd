namespace Infrastructure.Ef;

using Domain.Entities;

public interface IAddNewEvent
{
    Task Process(EventModel eventData);
}