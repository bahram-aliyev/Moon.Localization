using System.Collections.Generic;
using System.Globalization;

namespace Moon.Localization
{
    /// <summary>
    /// Defines basic contract for resource dictionaries.
    /// </summary>
    public interface IResourceDictionary
    {
        /// <summary>
        /// Gets the culture of the dictionary.
        /// </summary>
        CultureInfo Culture { get; }

        /// <summary>
        /// Gets a collection of localized values.
        /// </summary>
        IDictionary<string, string> Values { get; }
    }
}