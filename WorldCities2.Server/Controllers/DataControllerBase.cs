namespace WorldCities2.Server.Controllers
{
    /// <summary>
    /// TODO: Create an abstract base class for data controllers so we don't have to repeat a bunch of code.
    /// </summary>
    public class DataControllerBase
    {
        /* Rough sketch of base code
         
        public class BaseController<T> : Controller where T : class
{
    protected readonly DbContext _context;

    public BaseController(DbContext context)
    {
        _context = context;
    }

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
}
         
         */

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
