using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Data.GraphQL
{
    /// <summary>
    /// Represents a GraphQL query. This class allows for reading data from our DB.
    /// </summary>
    public class Query
    {
        #region Public Methods

        /// <summary>
        /// Gets all cities from the datatbase.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [Serial]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<City> GetCities([Service] ApplicationDbContext context)
        {
            return context.Cities;
        }

        /// <summary>
        /// Gets all countries from the database.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [Serial]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Country> GetCountries([Service] ApplicationDbContext context)
        {
            return context.Countries;
        }

        #endregion
    }
}
