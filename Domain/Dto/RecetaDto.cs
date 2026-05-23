namespace Domain.Dto;

public record RecetaDto(
    int Id,
    string Nombre,
    string? ImagenUrl,
    int? Nivel,
    string? Url);
