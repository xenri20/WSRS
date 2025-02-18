namespace WSRS_SWAFO.Helpers
{
    /// <summary>
    /// A helper class for string manipulation
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Truncate a string with an ellipsis if it exceeds the maximum length
        /// </summary>
        /// <param name="input">string to truncate</param>
        /// <param name="maxLength">Determines max length for the input</param>
        /// <returns>Truncated input text if it reaches the max length</returns>
        public static string TruncateWithEllipsis(string input, int maxLength = 32)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
            {
                return input;
            }

            return input.Substring(0, maxLength) + "...";
        }
    }
}
