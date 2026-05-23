namespace Domain.Dto;

public record RecetaCraftedItemDto(
    int Id,
    string Nombre,
    string? ImagenUrl,
    char? Grado);
