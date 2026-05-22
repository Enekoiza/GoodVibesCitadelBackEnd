namespace ApplicationService;

using Domain.Dto;

public interface IGetAllEventsProcessor
{
    public Task<List<EventDto>> Process();
}