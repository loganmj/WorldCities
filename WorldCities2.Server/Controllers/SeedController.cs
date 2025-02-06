using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize(Roles = "Administrator")]
    public class SeedController(ApplicationDbContext context,
                                IWebHostEnvironment environment,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<ApplicationUser> userManager,
                                IConfiguration configuration) : ControllerBase
    {
        #region Fields

        private readonly ApplicationDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;

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

        /// <summary>
        /// Creates a table with default users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            // Setup default role names
            var role_RegisteredUser = "RegisteredUser";
            var role_Administrator = "Administrator";

            // Create the default roles if they do not exist
            if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null) 
            {
                await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
            }

            if (await _roleManager.FindByNameAsync(role_Administrator) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role_Administrator));
            }

            // Create a list to track the newly added users
            var addedUserList = new List<ApplicationUser>();

            // Check if the admin user already exists
            var email_Admin = "admin@email.com";

            if (await _userManager.FindByNameAsync(email_Admin) == null) 
            {
                // Create a new admin account
                var user_Admin = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_Admin,
                    Email = email_Admin
                };

                // Insert admin user into the database
                await _userManager.CreateAsync(user_Admin, _configuration["DefaultPasswords:Administrator"]!);

                // Assign the RegisteredUser and Administrator roles
                await _userManager.AddToRoleAsync(user_Admin, role_Administrator);
                await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);

                // Confirm the email and remove lockout
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;

                // Add th admin user to the added users list
                addedUserList.Add(user_Admin);
            }

            // Check if the standard user already exists
            var email_User = "user@email.com";

            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                // Create a new user account
                var user_User = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_User,
                    Email = email_User
                };

                // Insert admin user into the database
                await _userManager.CreateAsync(user_User, _configuration["DefaultPasswords:RegisteredUser"]!);

                // Assign the RegisteredUser role
                await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

                // Confirm the email and remove lockout
                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled = false;

                // Add th admin user to the added users list
                addedUserList.Add(user_User);
            }

            // If we added at least one user, persist changes to the database
            if (addedUserList.Count > 0) 
            {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new 
            {
                Count = addedUserList.Count,
                Users = addedUserList
            });
        }

        #endregion
    }
}