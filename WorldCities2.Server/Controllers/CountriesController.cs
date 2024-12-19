using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities2.Server.Data;
using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Controllers
{
    /// <summary>
    /// An entity controller for Country data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : DataControllerBase<Country>
    {
        #region Constructors

        /// <summary>
        /// Constructs a controller with the specified database context.
        /// </summary>
        /// <param name="context"></param>
        public CountriesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

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

        #endregion
    }
}