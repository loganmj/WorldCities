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

        /// <summary>
        /// Handles a user request to register a new account.
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        public async Task<IActionResult> Register(APIRegisterRequest registerRequest) 
        {
            // Create a new application user object
            var user = new ApplicationUser
            {
                Email = registerRequest.Email
            };

            // Call the user manager API to attempt to create the new user
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            // If creation failed, return error
            if (!result.Succeeded) 
            {
                return BadRequest(new APIRegisterResult
                {
                    Success = false,
                    Message = "Failed to registure new user.",
                    Errors = result.Errors.Select(error => error.Description).ToList()
                });
            }

            // Handle successful registration
            return Ok(new APIRegisterResult { Success = true, Message = "New user created."});
        }

        #endregion
    }
}
