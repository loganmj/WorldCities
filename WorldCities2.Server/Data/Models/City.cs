using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities2.Server.Data.Models;

/// <summary>
/// Data model for a City.
/// </summary>
[Table("Cities")]
[Index(nameof(Name))]
[Index(nameof(Latitude))]
[Index(nameof(Longitude))]
public class City
{
    #region Properties

    /// <summary>
    /// The unique id and primary key for this city.
    /// </summary>
    [Key]
    [Required]
    public int ID { get; set; }

    /// <summary>
    /// The name of the city (in UTF8 format).
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The latitude of the city.
    /// </summary>
    [Column(TypeName = "decimal(7,4)")]
    public decimal Latitude { get; set; }

    /// <summary>
    /// The longitude of the city.
    /// </summary>
    [Column(TypeName = "decimal(7,4")]
    public decimal Longitude { get; set; }

    /// <summary>
    /// The id of the associated country.
    /// Used as a foreign key.
    /// </summary>
    [ForeignKey(nameof(Country))]
    public int CountryId { get; set; }

    #endregion
}