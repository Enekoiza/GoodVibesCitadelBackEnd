namespace Infrastructure.Ef;

using Domain.Models;

public interface IAddNewEvent
{
    Task Process(EventModel eventData);
}