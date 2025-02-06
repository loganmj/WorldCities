using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WorldCities2.Server.Data;
using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Controllers
{
    /// <summary>
    /// A controller that fascilitates user login requests.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(ApplicationDbContext context, UserManager<ApplicationUser> user, JWTHandler jwtHandler) : ControllerBase
    {
        #region Fields

        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = user;
        private readonly JWTHandler _jwtHandler = jwtHandler;

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles a login POST request.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(APILoginRequest loginRequest) 
        {
            // Determine user from login request
            var user = await _userManager.FindByNameAsync(loginRequest.Email);

            // Invalid login
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password)) 
            {
                return Unauthorized(new APILoginResult()
                {
                    Success = false,
                    Message = "Invalid Email or Password."
                }); 
            }

            // Successful login
            var secToken = await _jwtHandler.GetTokenAsync(user);
            var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
            return Ok(new APILoginResult() { Success = true, Message = "Login successful", Token = jwt });
        }

        #endregion
    }
}
