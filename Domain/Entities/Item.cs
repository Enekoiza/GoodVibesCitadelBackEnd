namespace Domain.Entities;

public class Item
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? ImagenUrl { get; set; }

    public char? Grado { get; set; }

    public virtual ICollection<ItemReceta> ItemRecetas { get; set; } = new List<ItemReceta>();
}
