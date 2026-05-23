namespace Domain.Dto;

public record RecetaMaterialesResponseDto(
    IReadOnlyList<RecetaMaterialDto> Materiales,
    IReadOnlyList<RecetaCraftedItemDto> Items);
