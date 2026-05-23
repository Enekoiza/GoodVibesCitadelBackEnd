namespace Domain.Entities;

public class ItemReceta
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int RecetaId { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Receta Receta { get; set; } = null!;
}
