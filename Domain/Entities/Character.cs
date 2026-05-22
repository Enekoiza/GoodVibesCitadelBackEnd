namespace Domain.Entities;

using GoodVibesCitadelBackend.Models;

public class Character
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long Classid { get; set; }

    public string Userid { get; set; } = null!;

    public int Level { get; set; }

    public string? Login { get; set; } = string.Empty;

    public string? Password { get; set; } = string.Empty;

    public virtual Class Class { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}