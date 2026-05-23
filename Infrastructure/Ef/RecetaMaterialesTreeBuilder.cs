namespace Infrastructure.Ef;

using Domain.Dto;
using Domain.Entities;

internal static class RecetaMaterialesTreeBuilder
{
    public static IReadOnlyList<RecetaMaterialDto> Build(IReadOnlyList<RecetaMaterial> materiales)
    {
        var hijosPorPadre = materiales
            .Where(m => m.ParentId.HasValue)
            .GroupBy(m => m.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var raices = materiales.Where(m => m.ParentId is null).ToList();

        return raices.Select(m => MapMaterial(m, hijosPorPadre, 0)).ToList();
    }

    private static RecetaMaterialDto MapMaterial(
        RecetaMaterial recetaMaterial,
        Dictionary<int, List<RecetaMaterial>> hijosPorPadre,
        int nivel)
    {
        var hijos = hijosPorPadre.TryGetValue(recetaMaterial.Id, out var children)
            ? children.Select(h => MapMaterial(h, hijosPorPadre, nivel + 1)).ToList()
            : [];

        return new RecetaMaterialDto(
            recetaMaterial.Id,
            recetaMaterial.Material?.Nombre,
            recetaMaterial.Material?.ImagenUrl,
            recetaMaterial.Cantidad,
            nivel,
            hijos);
    }
}
