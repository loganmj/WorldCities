using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities2.Server.Data.Models
{
    /// <summary>
    /// Data model for a City.
    /// </summary>
    public class City
    {
        #region Properties

        /// <summary>
        /// The unique identifier value for the city.
        /// </summary>
        [Key]
        [Required]
        public uint Id { get; set; }

        /// <summary>
        /// The name of the city.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The latitude value of the City.
        /// </summary>
        [Column(TypeName ="decimal(7,4)")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// The longitude value of the City.
        /// </summary>
        [Column(TypeName = "decimal(7,4)")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// The identifier for the Country that this City is associated with.
        /// This is implemented as a foreign key in the database.
        /// </summary>
        public uint CountryID { get; set; }

        #endregion
    }
}
