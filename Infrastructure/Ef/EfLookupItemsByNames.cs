namespace Infrastructure.Ef;

using Domain.Dto;
using Microsoft.EntityFrameworkCore;

public class EfLookupItemsByNames(AppDbContext db) : ILookupItemsByNames
{
    public async Task<List<ItemLookupDto>> Process(IReadOnlyList<string> names)
    {
        var normalized = names
            .Select(name => name.Trim())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalized.Count == 0)
        {
            return [];
        }

        var lowerNames = normalized.Select(name => name.ToLower()).ToList();

        var items = await db.Items
            .AsNoTracking()
            .Where(item => lowerNames.Contains(item.Nombre.ToLower()))
            .Select(item => new ItemLookupDto(item.Nombre, item.ImagenUrl))
            .ToListAsync();

        var imageByName = items.ToDictionary(
            item => item.Name,
            item => item.ImageUrl,
            StringComparer.OrdinalIgnoreCase);

        var missingNames = normalized
            .Where(name => !imageByName.ContainsKey(name))
            .Select(name => name.ToLower())
            .ToList();

        if (missingNames.Count > 0)
        {
            var materials = await db.Materiales
                .AsNoTracking()
                .Where(material => missingNames.Contains(material.Nombre.ToLower()))
                .Select(material => new ItemLookupDto(material.Nombre, material.ImagenUrl))
                .ToListAsync();

            foreach (var material in materials)
            {
                imageByName.TryAdd(material.Name, material.ImageUrl);
            }
        }

        return normalized
            .Select(name => new ItemLookupDto(name, imageByName.GetValueOrDefault(name)))
            .ToList();
    }
}
