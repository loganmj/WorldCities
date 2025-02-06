using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Data;

/// <summary>
/// Database context
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    #region Constructors

    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    public ApplicationDbContext() : base() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="options"></param>
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    #endregion

    #region Public Methods

    /// <summary>
    /// Represents the Cities database table.
    /// </summary>
    public DbSet<City> Cities => Set<City>();

    /// <summary>
    /// Represents the Countries database table.
    /// </summary>
    public DbSet<Country> Countries => Set<Country>();

    #endregion
}