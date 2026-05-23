namespace Domain.Entities;

public class Receta
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? ImagenUrl { get; set; }

    public int? Nivel { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<ItemReceta> ItemRecetas { get; set; } = new List<ItemReceta>();

    public virtual ICollection<RecetaMaterial> Materiales { get; set; } = new List<RecetaMaterial>();
}
