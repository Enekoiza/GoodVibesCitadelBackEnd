namespace Infrastructure.Ef;

using Domain.Models;

public interface IGetAllPartyCompositions
{
    Task<List<CompositionModel>> Process();
}