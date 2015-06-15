using System;

namespace Moon.Localization
{
    /// <summary>
    /// The exception thrown when <see cref="IResourceDictionary" /> for current culture does not exist.
    /// </summary>
    public class DictionaryNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryNotFoundException" /> class.
        /// </summary>
        public DictionaryNotFoundException()
            : base("A resource dictionary for current culture could not be found.")
        {
        }
    }
}