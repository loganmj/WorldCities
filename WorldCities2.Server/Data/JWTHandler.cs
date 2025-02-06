using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Data
{
    /// <summary>
    /// A Service class that handles JWT tokens.
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    public class JWTHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        #region Fields
        
        private readonly IConfiguration _configuration = configuration;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves JWT signing credentials.
        /// </summary>
        /// <returns>A SigningCredentials object built from the configured security key value.</returns>
        private SigningCredentials GetSigningCredentials() 
        {
            // Get the secret key value from the configuration.
            var key = _configuration["JwtSettings:SecurityKey"];

            if (key == null) 
            {
                throw new ArgumentNullException($"The configured security key is null.", nameof(key));
            }

            if (key.Length < 32)
            {
                throw new ArgumentException($"The configured security key must be at least 32 characters long.", nameof(key));
            }

            // Created a signing credentials object from the key value
            return new SigningCredentials(new SymmetricSecurityKey((byte[]?)Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        /// Get claims
        /// </summary>
        /// <returns>A a list of identity claims.</returns>
        private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            if (user == null) 
            {
                return [];
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, string.IsNullOrEmpty(user.Email) ? "" : user.Email)
            };

            foreach (var role in await _userManager.GetRolesAsync(user)) 
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a JWT token.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<JwtSecurityToken> GetTokenAsync(ApplicationUser user)
        {
            var jwt = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: await GetClaimsAsync(user),
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationTimeInMinutes"])
                ),
                signingCredentials: GetSigningCredentials());

            return jwt;
        }

        #endregion
    }
}
