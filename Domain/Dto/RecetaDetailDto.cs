namespace Domain.Dto;

public record RecetaDetailDto(
    int Id,
    string Nombre,
    string? ImagenUrl,
    int? Nivel,
    string? Url,
    IReadOnlyList<RecetaMaterialDto> Materiales);
