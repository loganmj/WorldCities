using Microsoft.EntityFrameworkCore;
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

        [Serial]
        public async Task<APIResult<CityDTO>> GetCitiesApiResult([Service] ApplicationDbContext context,
                                                                 int pageIndex = 0,
                                                                 int pageSize = 10,
                                                                 string? sortColumn = null,
                                                                 string? sortOrder = null,
                                                                 string? filterColumn = null,
                                                                 string? filterQuery = null)
        {
            return await APIResult<CityDTO>.CreateAsync(context.Cities.AsNoTracking()
                .Select((city => new CityDTO
                {
                    ID = city.ID,
                    Name = city.Name,
                    Latitude = city.Latitude,
                    Longitude = city.Longitude,
                    CountryId = city.Country == null ? default : city.Country.Id,
                    Country = city.Country == null ? default : city.Country.Name
                })),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        #endregion
    }
}
