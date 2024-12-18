using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WorldCities2.Server.Data.Models;

/// <summary>
/// Data model for a country object.
/// </summary>
[Table("Countries")]
[Index(nameof(Name))]
[Index(nameof(ISO2))]
[Index(nameof(ISO3))]
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
    [JsonPropertyName("iso2")]
    public required string ISO2 { get; set; }

    /// <summary>
    /// The country code (in ISO 3166-1 ALPHA-3 format).
    /// </summary>
    [JsonPropertyName("iso3")]
    public required string ISO3 { get; set; }

    /// <summary>
    /// A collection of all the cities related to this country.
    /// </summary>
    public ICollection<City>? Cities { get; set; }

    #endregion
}