using Microsoft.EntityFrameworkCore;
using SuperHeroAPIs.Entities;

namespace SuperHeroAPIs.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<SuperHero> SuperHeroes { get; set; }
}
