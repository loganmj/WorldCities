using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using WorldCities2.Server.Data;
using WorldCities2.Server.Data.Models;

namespace WorldCities.Server.Controllers
{
    /// <summary>
    /// Used to seed our local database for dev testing.
    /// Note: There is a bug in the import method that isn't protecting the context from double-adding cities.
    /// My best guess is that it has something to do with type conversion when reading in the latitude and longitude
    /// decomal values from the Excel sheet.
    /// </summary>
    /// <remarks>
    /// Constructs a controller with a give DB context.
    /// </remarks>
    /// <param name="context"></param>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController(ApplicationDbContext context, IWebHostEnvironment environment) : ControllerBase
    {
        #region Fields

        private readonly ApplicationDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;

        #endregion

        #region Public Methods

        /// <summary>
        /// Imports data from seed file to populate the development database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Import()
        {
            // Check for dev environment.
            if (!_environment.IsDevelopment())
            {
                throw new SecurityException("Not allowed outside of development environment.");
            }

            // Get path to seed file
            var path = Path.Combine(_environment.ContentRootPath, "Data/Source/worldcities.xlsx");

            // Construct the file stream and wrap it in our excel package class.
            using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);

            // Get the first worksheet
            var worksheet = excelPackage.Workbook.Worksheets.First();

            // Define how many rows we want to prcess
            var endRowNumber = worksheet.Dimension.End.Row;

            // Initialize the record counters
            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            // Create a lookup table to cache all the countries that already exist in the database.
            // This prevents us from adding countries to the context that already exist in the database.
            // It also prevents us from needing to query the database more than once.
            var countriesCacheTable = _context.Countries.AsNoTracking().ToDictionary(country => country.Name, StringComparer.OrdinalIgnoreCase);

            // Iterate through all rows, skipping the first one (the title row)
            for (int rowNumber = 2; rowNumber <= endRowNumber; rowNumber++)
            {
                // Get the row as an array of cells.
                // This code grabs the rows starting and ending with [current row]
                // and the columns starting with 1 and ending with [end column].
                var rowArray = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];

                // Get country data from the cells
                var countryName = rowArray[rowNumber, 5].GetValue<string>();
                var iso2 = rowArray[rowNumber, 6].GetValue<string>();
                var iso3 = rowArray[rowNumber, 7].GetValue<string>();

                // Skip this country if it already exists in the database
                if (countriesCacheTable.ContainsKey(countryName)) continue;

                // Create the Country entity and fill it with the Excel data
                var country = new Country
                {
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3
                };

                // Add the entity to the database context
                await _context.Countries.AddAsync(country);

                // Store the entity in our lookup table
                countriesCacheTable.Add(countryName, country);

                // Increment the countries counter
                numberOfCountriesAdded++;
            }

            // Save all the countries into the database
            if (numberOfCountriesAdded > 0)
            {
                await _context.SaveChangesAsync();
            }

            // Create another cache table for the city entities
            var citiesCacheTable = _context.Cities.AsNoTracking().ToDictionary(city => (city.Name, city.Latitude, city.Longitude, city.CountryId));

            // Iterate through all rows, skipping the first one (the title row)
            for (int rowNumber = 2; rowNumber <= endRowNumber; rowNumber++)
            {
                // Get the row as an array of cells.
                // This code grabs the rows starting and ending with [current row]
                // and the columns starting with 1 and ending with [end column].
                var rowArray = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];

                // Get city data from the cells
                var cityName = rowArray[rowNumber, 1].GetValue<string>();
                var latitude = rowArray[rowNumber, 3].GetValue<decimal>();
                var longitude = rowArray[rowNumber, 4].GetValue<decimal>();
                var countryName = rowArray[rowNumber, 5].GetValue<string>();

                // Retrieve country ID by country name from the lookup table
                // This way we don't have to query the database again
                var countryId = countriesCacheTable[countryName].Id;

                // Skip this city if it already exists in the database
                if (citiesCacheTable.ContainsKey((cityName, latitude, longitude, countryId))) continue;

                // Create the City entity and fill it with the Excel data
                var city = new City
                {
                    Name = cityName,
                    Latitude = latitude,
                    Longitude = longitude,
                    CountryId = countryId
                };

                // Add the entity to the database context
                await _context.Cities.AddAsync(city);

                // Store the entity in our lookup table
                citiesCacheTable.Add((cityName, latitude, longitude, countryId), city);

                // Increment the countries counter
                numberOfCitiesAdded++;
            }

            // Save changes to the context
            if (numberOfCitiesAdded > 0)
            {
                await _context.SaveChangesAsync();
            }

            // Return a result object containing the the number of countries and cities that we added.
            return new JsonResult(new
            {
                Countries = numberOfCountriesAdded,
                Cities = numberOfCitiesAdded
            });
        }

        #endregion
    }
}