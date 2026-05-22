namespace ApplicationService;

using Domain.Models;
using ErrorOr;

public interface IAddNewEventPartyCombination
{
    public Task<ErrorOr<Success>> Process(EventPartyModel eventPartyModel);
}