using System;

namespace Moon.Localization
{
    /// <summary>
    /// The exception thrown when cultures of <see cref="IResourceDictionary" /> added to
    /// <see cref="AggregateDictionary" /> do not match.
    /// </summary>
    public class CultureDoesNotMatchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CultureDoesNotMatchException" /> class.
        /// </summary>
        public CultureDoesNotMatchException()
            : base("The added dictionary is for different culture than the aggregate dictionary.")
        {
        }
    }
}