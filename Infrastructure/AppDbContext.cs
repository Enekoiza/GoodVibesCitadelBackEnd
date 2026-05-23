namespace Infrastructure;

using Domain.Entities;
using GoodVibesCitadelBackend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Character> Characters { get; set; }

    public DbSet<Composition> Compositions { get; set; }

    public DbSet<Class> Classes { get; set; }

    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Class>().ToTable("class");
        modelBuilder.Entity<Character>(entity =>
        {
            entity.ToTable("characters");
            entity.HasIndex(c => c.Name).IsUnique();
        });
        modelBuilder.Entity<Composition>().ToTable("composition");

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("event");

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            entity.Property(e => e.PartyComposition)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<List<PartyCompositionEntity>>(v, jsonOptions) 
                         ?? new List<PartyCompositionEntity>()
                )
                .HasColumnType("json");
        });
    }
}