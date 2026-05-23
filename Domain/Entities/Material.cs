namespace Domain.Entities;

public class Material
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? ImagenUrl { get; set; }

    public virtual ICollection<RecetaMaterial> RecetaMateriales { get; set; } = new List<RecetaMaterial>();
}
