using System.ComponentModel.DataAnnotations;

namespace WorldCities2.Server.Data
{
    /// <summary>
    /// A data transfer object for a user registration request.
    /// </summary>
    public class APIRegisterRequest
    {
        #region Properties

        /// <summary>
        /// The user's email address.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; }

        /// <summary>
        /// The user's password.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }

        #endregion
    }
}
