using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Data.GraphQL
{
    /// <summary>
    /// Implements a GraphQL mutation. This allows us to write to our database.
    /// </summary>
    public class Mutation
    {
        #region Public Methods

        /// <summary>
        /// Adds a new city.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cityDTO"></param>
        /// <returns></returns>
        [Serial]
        [Authorize(Roles = ["RegisteredUser"])]
        public async Task<City> AddCity([Service] ApplicationDbContext context, CityDTO cityDTO) 
        {
            // Create new city entity object from given data
            var city = new City()
            {
                Name = cityDTO.Name ?? "",
                Latitude = cityDTO.Latitude,
                Longitude = cityDTO.Longitude,
                CountryId = cityDTO.CountryId
            };

            // Add the new city and save changes
            context.Cities.Add(city);
            await context.SaveChangesAsync();
            return city;
        }

        /// <summary>
        /// Update an existing city.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cityDTO"></param>
        /// <returns></returns>
        [Serial]
        [Authorize(Roles = ["RegisteredUser"])]
        public async Task<City> UpdateCity([Service] ApplicationDbContext context, CityDTO cityDTO)
        {
            // Try to retrieve the given city from the database
            var city = await context.Cities
                .Where(entity => entity.ID == cityDTO.ID)
                .FirstOrDefaultAsync();

            // If we can't retrieve a city with the given data, return
            if (city == null) 
            {
                // TODO: Handle errors
                throw new NotSupportedException();
            }

            // Update the city object
            city.Name = cityDTO.Name ?? "";
            city.Latitude = cityDTO.Latitude;
            city.Longitude = cityDTO.Longitude;
            city.CountryId = cityDTO.CountryId;

            // Save changes
            context.Cities.Update(city);
            await context.SaveChangesAsync();
            return city;
        }

        /// <summary>
        /// Delete a city
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Serial]
        [Authorize(Roles = ["Administrator"])]
        public async Task DeleteCity([Service] ApplicationDbContext context, int id)
        {
            // Try to retrieve the given city from the database
            var city = await context.Cities
                .Where(entity => entity.ID == id)
                .FirstOrDefaultAsync();

            // If we can't retrieve a city with the given data, return
            if (city == null)
            {
                // TODO: Handle errors
                return;
            }

            // Delete the city object and save changes
            context.Cities.Remove(city);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a new country.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="countryDTO"></param>
        /// <returns></returns>
        [Serial]
        [Authorize(Roles = ["RegisteredUser"])]
        public async Task<Country> AddCountry([Service] ApplicationDbContext context, CountryDTO countryDTO)
        {
            // Create new country entity object from given data
            var country = new Country()
            {
                Name = countryDTO.Name ?? "",
                ISO2 = countryDTO.ISO2,
                ISO3 = countryDTO.ISO3
            };

            // Add the new country and save changes
            context.Countries.Add(country);
            await context.SaveChangesAsync();
            return country;
        }

        /// <summary>
        /// Update an existing country
        /// </summary>
        /// <param name="context"></param>
        /// <param name="countryDTO"></param>
        /// <returns></returns>
        [Serial]
        [Authorize(Roles = ["RegisteredUser"])]
        public async Task<Country> UpdateCountry([Service] ApplicationDbContext context, CountryDTO countryDTO)
        {
            // Try to retrieve the given country from the database.
            var country = await context.Countries
                .Where(entity => entity.Id == countryDTO.Id)
                .FirstOrDefaultAsync();

            if (country == null) 
            {
                // TODO: Handle errors
                throw new NotSupportedException();
            }

            // Update country data
            country.Name = countryDTO.Name ?? "";
            country.ISO2 = countryDTO.ISO2;
            country.ISO3 = countryDTO.ISO3;

            // Save changes
            context.Update(country);
            await context.SaveChangesAsync();
            return country;
        }


        /// <summary>
        /// Delete a country
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Serial]
        [Authorize(Roles = ["Administrator"])]
        public async Task DeleteCountry([Service] ApplicationDbContext context, int id) 
        {
            // Try to retrieve the given country from the database.
            var country = await context.Countries
                .Where(entity => entity.Id == id)
                .FirstOrDefaultAsync();

            if (country == null)
            {
                // TODO: Handle errors
                throw new NotSupportedException();
            }

            context.Remove(country);
            await context.SaveChangesAsync();
        }

        #endregion
    }
}
