namespace Infrastructure.Ef;

using Domain.Dto;

public interface IAddNewEvent
{
    Task Process(EventModel eventData);
}