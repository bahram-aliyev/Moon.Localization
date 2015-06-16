using Moon.Localization.Json;

namespace Moon.Localization
{
    /// <summary>
    /// <see cref="DictionaryLoader" /> extension methods.
    /// </summary>
    public static class DictionaryLoaderExtensions
    {
        /// <summary>
        /// Loads JSON dictionaries in the specified folder (including subfolders).
        /// </summary>
        /// <param name="folderPath">The path where to look for dictionaries.</param>
        /// <param name="searchPattern">The string to match against the names of files in path.</param>
        /// <param name="loader">A function used to load dictionaries.</param>
        public static void LoadJson(this DictionaryLoader loader, string folderPath)
            => loader.Load(folderPath, "*.json", JsonDictionary.Load);
    }
}