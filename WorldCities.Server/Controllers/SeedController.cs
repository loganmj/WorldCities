using Microsoft.AspNetCore.Mvc;
using WorldCities.Server.Data;

namespace WorldCities.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        #region Fields

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a controller with a give DB context.
        /// </summary>
        /// <param name="context"></param>
        public SeedController(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion
    }
}
