using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WorldCities2.Server.Data.Models
{
    /// <summary>
    /// Data transfer object for city data.
    /// </summary>
    public class CityDTO
    {
        #region Properties

        /// <summary>
        /// The unique id and primary key for this city.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the city (in UTF8 format).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The latitude of the city.
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// The longitude of the city.
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// The id of the associated country.
        /// Used as a foreign key.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// The name of the country in which the city resides.
        /// </summary>
        public string? Country { get; set; }


        #endregion
    }
}
