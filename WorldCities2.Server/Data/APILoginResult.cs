namespace WorldCities2.Server.Data
{
    /// <summary>
    /// A data transfer object for user login result data.
    /// </summary>
    public class APILoginResult
    {
        #region Properties

        /// <summary>
        /// True if the login attempt is successful, False otherwise.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Login result message.
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// The JWT token if the login attempt is successful, or NULL if the login failed.
        /// </summary>
        public string? Token { get; set; }

        #endregion
    }
}
