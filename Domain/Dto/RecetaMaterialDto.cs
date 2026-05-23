namespace Domain.Dto;

public record RecetaMaterialDto(
    int Id,
    string? Nombre,
    string? ImagenUrl,
    decimal? Cantidad,
    int Nivel,
    IReadOnlyList<RecetaMaterialDto> Hijos);
