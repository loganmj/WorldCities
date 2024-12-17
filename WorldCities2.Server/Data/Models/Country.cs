using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities2.Server.Data.Models;

/// <summary>
/// Data model for a country object.
/// </summary>
[Table("Countries")]
[Index(nameof(Name))]
[Index(nameof(IS02))]
[Index(nameof(IS03))]
public class Country
{
    #region Properties

    /// <summary>
    /// The unique id and primary key for this country.
    /// </summary>
    [Key]
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// The country name (in UTF8 format).
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The country code (in ISO 3166-1 ALPHA-2 format).
    /// </summary>
    public required string IS02 { get; set; }

    /// <summary>
    /// The country code (in ISO 3166-1 ALPHA-3 format).
    /// </summary>
    public required string IS03 { get; set; }

    /// <summary>
    /// A collection of all the cities related to this country.
    /// </summary>
    public ICollection<City>? Cities { get; set; }

    #endregion
}