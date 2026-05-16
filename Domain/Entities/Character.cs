namespace Domain.Entities;

using GoodVibesCitadelBackend.Models;

public class Character
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long Classid { get; set; }

    public string Userid { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}