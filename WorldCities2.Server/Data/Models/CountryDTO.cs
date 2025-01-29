using System.Text.Json.Serialization;

namespace WorldCities2.Server.Data.Models
{
    /// <summary>
    /// A country data transfer object that carries the desired country data to the front end.
    /// </summary>
    public class CountryDTO
    {
        #region Properties

        /// <summary>
        /// The unique id and primary key for this country.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The country name (in UTF8 format).
        /// </summary>
        public string Name { get; set; }

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
        /// The number of cities listed for the country.
        /// </summary>
        public int? NumberOfCities { get; set; } = null;

        #endregion
    }
}
