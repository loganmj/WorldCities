using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data.Models;

namespace WorldCities.Server.Data;

/// <summary>
/// Database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() : base() { }
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
    public DbSet<City> Cities => Set<City>();
    public DbSet<Country> Countries => Set<Country>();
}
