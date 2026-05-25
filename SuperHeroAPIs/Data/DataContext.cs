using Microsoft.EntityFrameworkCore;
using SuperHeroAPIs.Entities;

namespace SuperHeroAPIs.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<SuperHero> SuperHeroes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SuperHero>().HasData(
            new SuperHero { Id = 1, Name = "Spider-Man", FirstName = "Peter", LastName = "Parker", Place = "New York" },
            new SuperHero { Id = 2, Name = "Iron Man", FirstName = "Tony", LastName = "Stark", Place = "Malibu" },
            new SuperHero { Id = 3, Name = "Thor", FirstName = "Thor", LastName = "Odinson", Place = "Asgard" }
        );
    }
}
