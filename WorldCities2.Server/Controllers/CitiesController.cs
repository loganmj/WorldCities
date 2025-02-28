﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities2.Server.Data;
using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Controllers
{
    /// <summary>
    /// An API controller for city data
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController(ApplicationDbContext context) : ControllerBase
    {
        #region Fields
        
        private ApplicationDbContext _context = context;
        
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
        /// Gets all object data.
        /// Allows for sorting, filtering, and pagination.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <param name="filterColumn"></param>
        /// <param name="filterQuery"></param>
        /// <returns>Returns an APIResult containing the result data.</returns>
        [HttpGet]
        public async Task<ActionResult<APIResult<CityDTO>>> GetCities(int pageIndex = 0,
                                                                      int pageSize = 10,
                                                                      string? sortColumn = null,
                                                                      string? sortOrder = null,
                                                                      string? filterColumn = null,
                                                                      string? filterQuery = null) 
        {
            try
            {
                return await APIResult<CityDTO>.CreateAsync(
                    _context.Cities.AsNoTracking()
                    .Select((city) => new CityDTO
                    {
                        ID = city.ID,
                        Name = city.Name,
                        Latitude = city.Latitude,
                        Longitude = city.Longitude,
                        CountryId = city.CountryId,
                        Country = city.Country == null ? "--" : city.Country.Name
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
                return StatusCode(500, "Internal server error");
            }
        }

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
        [Authorize(Roles ="RegisteredUser")]
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
        [Authorize(Roles = "RegisteredUser")]
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
        [Authorize(Roles = "Administrator")]
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