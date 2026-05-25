namespace Domain.Dto;

public record CpWarehouseEntryDto(
    int Id,
    string EntryType,
    int EntityId,
    string Nombre,
    string? ImagenUrl,
    int Quantity);

public record AddCpWarehouseEntryDto(
    string EntryType,
    int EntityId,
    int Quantity);

public record UpdateCpWarehouseEntryDto(int Quantity);

public record SyncCpWarehouseEntryDto(
    int? Id,
    string EntryType,
    int EntityId,
    int Quantity);

public record SyncCpWarehouseDto(IReadOnlyList<SyncCpWarehouseEntryDto> Entries);

public record WarehouseCatalogResultDto(
    int Id,
    string Nombre,
    string? ImagenUrl,
    string EntryType);
