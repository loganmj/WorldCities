using System.Web;

namespace WorldCities2.Server.Security
{
    /// <summary>
    /// A static class containing logic for sanitizing inputs.
    /// </summary>
    public static class Sanitizer
    {
        #region Public Methods

        /// <summary>
        /// Sanitizes a string value to remove potentially harmful characters.
        /// This is check against injection style attacks.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string? SanitizeString(string? value)
        {
            Console.WriteLine($"Data value before sanitizing: {value}");
            var sanitizedValue = HttpUtility.HtmlEncode(value);
            Console.WriteLine($"Data value after sanitizing: {sanitizedValue}");
            return sanitizedValue;
        }

        #endregion
    }
}
