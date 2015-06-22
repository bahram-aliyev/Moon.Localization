using System;
using System.IO;

namespace Moon.Localization
{
    /// <summary>
    /// Loads dictionaries from various sources.
    /// </summary>
    public class DictionaryLoader
    {
        readonly string rootPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryLoader" /> class.
        /// </summary>
        /// <param name="rootPath">The root / application path.</param>
        public DictionaryLoader(string rootPath)
        {
            Requires.NotNullOrWhiteSpace(rootPath, nameof(rootPath));

            this.rootPath = rootPath;
        }

        /// <summary>
        /// Loads dictionaries that match a search pattern in the specified folder (including subfolders).
        /// </summary>
        /// <param name="folderPath">The path where to look for dictionaries.</param>
        /// <param name="searchPattern">The string to match against the names of files in path.</param>
        /// <param name="loader">A function used to load dictionaries.</param>
        public DictionaryLoader Load(string folderPath, string searchPattern, Func<Stream, IResourceDictionary> loader)
        {
            folderPath = ResolvePath(folderPath);

            foreach (var file in Directory.EnumerateFiles(folderPath, searchPattern, SearchOption.AllDirectories))
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    Resources.Load(loader(stream));
                }
            }

            return this;
        }

        string ResolvePath(string path)
        {
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri)
            {
                path = Path.Combine(rootPath, path);
            }

            return path;
        }
    }
}