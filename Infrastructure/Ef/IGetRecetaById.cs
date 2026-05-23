namespace Infrastructure.Ef;

using Domain.Dto;

public interface IGetRecetaById
{
    Task<RecetaDetailDto?> Process(int id);
}
