namespace WorldCities2.Server.Data
{
    /// <summary>
    /// A data transfer object for a user registration result.
    /// </summary>
    public class APIRegisterResult
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
        /// A list of request errors.
        /// The list is empty if the request had no errors.
        /// </summary>
        public List<string> Errors { get; set; } = [];

        #endregion
    }
}
