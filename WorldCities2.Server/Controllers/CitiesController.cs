using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities2.Server.Controllers;
using WorldCities2.Server.Data;
using WorldCities2.Server.Data.Models;

namespace WorldCities.Server.Controllers
{
    /// <summary>
    /// An API controller for city data
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : DataControllerBase<City>
    {
        #region Constructors

        /// <summary>
        /// Constructs a controller with the specified database context.
        /// </summary>
        /// <param name="context"></param>
        public CitiesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if a city exists in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.ID == id);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a city with the specified ID from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a JSON object containing a single City.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        /// <summary>
        /// Modifies a City with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="city"></param>
        /// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.ID)
            {
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
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
        /// Adds a new city to the database.
        /// </summary>
        /// <param name="city"></param>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.ID }, city);
        }

        /// <summary>
        /// Deletes a city with the specified ID from the database.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if the specified city exists in the database.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("IsDuplicateCity")]
        public bool IsDuplicateCity(City city) 
        {
            // Sanitize the city object
            APIResult<City>.SanitizeStringProperties(city);

            // Return a city that has all of the matching fields
            return _context.Cities.AsNoTracking()
                                  .Any(x => x.Name == city.Name
                                            && x.Latitude == city.Latitude
                                            && x.Longitude == city.Longitude
                                            && x.CountryId == city.CountryId
                                            && x.ID != city.ID);
        }

        #endregion
    }
}