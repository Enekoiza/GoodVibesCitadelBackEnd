namespace Infrastructure;

using Domain.Entities;
using GoodVibesCitadelBackend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Character> Characters { get; set; }
    public DbSet<Class> Classes { get; set; }
    
    public DbSet<Event> Events { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Class>().ToTable("class");
        modelBuilder.Entity<Character>().ToTable("characters");
        modelBuilder.Entity<Event>().ToTable("event");
    }
}