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

    public DbSet<Item> Items { get; set; }

    public DbSet<Receta> Recetas { get; set; }

    public DbSet<ItemReceta> ItemRecetas { get; set; }

    public DbSet<RecetaMaterial> RecetaMateriales { get; set; }

    public DbSet<Material> Materiales { get; set; }

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

            entity.Property(e => e.Drops)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<List<EventDropEntity>>(v, jsonOptions)
                         ?? new List<EventDropEntity>()
                )
                .HasColumnType("json");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items");
            entity.Property(e => e.Nombre).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ImagenUrl).HasColumnName("imagen_url").HasMaxLength(500);
            entity.Property(e => e.Grado).HasMaxLength(1).IsFixedLength();
        });

        modelBuilder.Entity<Receta>(entity =>
        {
            entity.ToTable("recetas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ImagenUrl).HasColumnName("imagen_url").HasMaxLength(500);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<ItemReceta>(entity =>
        {
            entity.ToTable("item_recetas");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.RecetaId).HasColumnName("receta_id");

            entity.HasOne(e => e.Item)
                .WithMany(i => i.ItemRecetas)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Receta)
                .WithMany(r => r.ItemRecetas)
                .HasForeignKey(e => e.RecetaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("materiales");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ImagenUrl).HasColumnName("imagen_url").HasMaxLength(500);
        });

        modelBuilder.Entity<RecetaMaterial>(entity =>
        {
            entity.ToTable("receta_materiales");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.RecetaId).HasColumnName("receta_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.Cantidad).HasPrecision(10, 4);

            entity.HasOne(e => e.Receta)
                .WithMany(r => r.Materiales)
                .HasForeignKey(e => e.RecetaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Material)
                .WithMany(m => m.RecetaMateriales)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Parent)
                .WithMany(m => m.Hijos)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}