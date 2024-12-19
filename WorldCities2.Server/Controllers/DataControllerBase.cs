using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities2.Server.Data;

namespace WorldCities2.Server.Controllers
{
    /// <summary>
    /// TODO: Create an abstract base class for data controllers so we don't have to repeat a bunch of code.
    /// </summary>
    public class DataControllerBase<T> : ControllerBase where T: class
    {
        #region Fields

        protected ApplicationDbContext _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a controller with an injected database context.
        /// </summary>
        /// <param name="context"></param>
        public DataControllerBase( ApplicationDbContext context)
        {
            _context = context;
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
        /// <returns>Returns an APIResult containing the result data.</returns>
        [HttpGet]
        public async Task<ActionResult<APIResult<T>>> GetItems(int pageIndex = 0, int pageSize = 10, string? sortColumn = null, string? sortOrder = null)
        {
            try
            {
                return await APIResult<T>.CreateAsync(_context.Set<T>().AsNoTracking(), pageIndex, pageSize, sortColumn, sortOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        /* Rouch sketch of inhereting code
         
        public class CitiesController : BaseController<City>
{
    public CitiesController(DbContext context) : base(context)
    {
    }

    // Additional methods specific to CitiesController
}

public class CountriesController : BaseController<Country>
{
    public CountriesController(DbContext context) : base(context)
    {
    }

    // Additional methods specific to CountriesController
}
         
         */
    }
}
