using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities2.Server.Data;

namespace WorldCities2.Server.Controllers
{
    /// <summary>
    /// TODO: Create an abstract base class for data controllers so we don't have to repeat a bunch of code.
    /// </summary>
    /// <remarks>
    /// Constructs a controller with an injected database context.
    /// </remarks>
    /// <param name="context"></param>
    public abstract class DataControllerBase<T>(ApplicationDbContext context) : ControllerBase where T : class
    {
        #region Fields

        protected ApplicationDbContext _context = context;

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
        public async Task<ActionResult<APIResult<T>>> GetItems(int pageIndex = 0,
                                                               int pageSize = 10,
                                                               string? sortColumn = null,
                                                               string? sortOrder = null,
                                                               string? filterColumn = null,
                                                               string? filterQuery = null)
        {
            try
            {
                return await APIResult<T>.CreateAsync(
                    _context.Set<T>().AsNoTracking(),
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

        #endregion
    }
}
