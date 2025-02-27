using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities2.Server.Data;
using WorldCities2.Server.Data.Models;
using System.Linq.Dynamic.Core;
using WorldCities2.Server.Security;
using Microsoft.AspNetCore.Authorization;

namespace WorldCities2.Server.Controllers
{
    /// <summary>
    /// An entity controller for Country data.
    /// </summary>
    /// <remarks>
    /// Constructs a controller with the specified database context.
    /// </remarks>
    /// <param name="context"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(ApplicationDbContext context) : ControllerBase
    {
        #region Properties

        private readonly ApplicationDbContext _context = context;
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if a country with the specified ID exists in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        [HttpGet]
        public async Task<ActionResult<APIResult<CountryDTO>>> GetItems(int pageIndex = 0,
                                                                        int pageSize = 10,
                                                                        string? sortColumn = null,
                                                                        string? sortOrder = null,
                                                                        string? filterColumn = null,
                                                                        string? filterQuery = null) 
        {
            try
            {
                // The select statement here is being used to transform normal country objects into country DTO objects.
                return await APIResult<CountryDTO>.CreateAsync(
                    _context.Countries.AsNoTracking()
                                      .Select(c => new CountryDTO
                                      {
                                          Id = c.Id,
                                          Name = c.Name,
                                          ISO2 = c.ISO2,
                                          ISO3 = c.ISO3,
                                          NumberOfCities = c.Cities == null ? 0 : c.Cities.Count
                                      }),
                    pageIndex,
                    pageSize,
                    sortColumn,
                    sortOrder,
                    filterColumn,
                    filterQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                // Note: This error status code is not particularly helpful
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a Country with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A JSON object containing a single country.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        /// <summary>
        /// Modifies a country with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="country"></param>
        [HttpPut("{id}")]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                return BadRequest();
            }

            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Adds a new country to the database.
        /// </summary>
        /// <param name="country"></param>
        [HttpPost]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        /// <summary>
        /// Deletes a country with the specified ID from the database.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks a country to make sure that none of its fields
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("IsDuplicateField")]
        public bool IsDuplicateField(int countryId, string fieldName, string fieldValue) 
        {
            // Sanitize the string inputs
            var sanitizedFieldName = Sanitizer.SanitizeString(fieldName);
            var sanitizedFieldValue = Sanitizer.SanitizeString(fieldValue);

            // Dynamic LINQ shenanigans
            return (APIResult<Country>.IsValidProperty(sanitizedFieldName)) && _context.Countries.Any($"{sanitizedFieldName} == @0 && ID != @1", sanitizedFieldValue, countryId);
        }

        #endregion
    }
}