namespace Domain.Entities;

public class RecetaMaterial
{
    public int Id { get; set; }

    public int RecetaId { get; set; }

    public int? ParentId { get; set; }

    public int MaterialId { get; set; }

    public decimal? Cantidad { get; set; }

    public virtual Receta Receta { get; set; } = null!;

    public virtual Material Material { get; set; } = null!;

    public virtual RecetaMaterial? Parent { get; set; }

    public virtual ICollection<RecetaMaterial> Hijos { get; set; } = new List<RecetaMaterial>();
}
