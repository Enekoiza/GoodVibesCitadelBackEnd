namespace GoodVibesCitadelBackend.Models;

using Domain.Entities;

public class Class
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
}
