namespace Infrastructure.Ef;

using Domain.Dto;

public interface IGetRecetaMateriales
{
    Task<RecetaMaterialesResponseDto?> Process(int recetaId);
}
