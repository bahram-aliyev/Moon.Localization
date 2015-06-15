using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace Moon.Localization
{
    /// <summary>
    /// Provides access to localized resources.
    /// </summary>
    public static class Resources
    {
        static readonly ConcurrentDictionary<CultureInfo, AggregateDictionary> dictionaries;
        static CultureInfo defaultCulture;

        /// <summary>
        /// Initializes the <see cref="Resources" /> class.
        /// </summary>
        static Resources()
        {
            dictionaries = new ConcurrentDictionary<CultureInfo, AggregateDictionary>();
            defaultCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// Gets or sets the default culture used when an <see cref="IResourceDictionary" /> for the
        /// current UI culture does not exist.
        /// </summary>
        public static CultureInfo DefaultCulture
        {
            get { return defaultCulture; }
            set
            {
                Requires.NotNull(value, nameof(value));
                defaultCulture = value;
            }
        }

        /// <summary>
        /// Gets an enumeration of cultures of all loaded dictionaries.
        /// </summary>
        public static IEnumerable<CultureInfo> Cultures
            => dictionaries.Keys;

        /// <summary>
        /// Gets the current UI culture.
        /// </summary>
        public static CultureInfo CurrentCulture
            => CultureInfo.CurrentUICulture;

        /// <summary>
        /// Returns resource with the given name; or <c>null</c> if the resource does not exist.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        public static string Get(string name)
        {
            Requires.NotNullOrWhiteSpace(name, nameof(name));

            name = name.Replace('/', ':');
            var dictionary = GetDictionary();

            if (dictionary.Values.ContainsKey(name))
            {
                return dictionary.Values[name];
            }

            return null;
        }

        /// <summary>
        /// Returns resource with the given name in the given category; or <c>null</c> if the
        /// resource does not exist.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="name">The name of the resource.</param>
        public static string Get(string categoryName, string name)
        {
            Requires.NotNullOrWhiteSpace(categoryName, nameof(categoryName));
            Requires.NotNullOrWhiteSpace(name, nameof(name));

            return Get($"{categoryName}:{name}") ?? Get(name);
        }

        /// <summary>
        /// Returns a resource dictionary for the current culture.
        /// </summary>
        public static IResourceDictionary GetDictionary()
        {
            var culture = CurrentCulture;

            if (!dictionaries.ContainsKey(culture))
            {
                culture = culture.Parent;
            }

            if (!dictionaries.ContainsKey(culture))
            {
                culture = defaultCulture;
            }

            if (!dictionaries.ContainsKey(culture))
            {
                throw new DictionaryNotFoundException();
            }

            return dictionaries[culture];
        }

        /// <summary>
        /// Loads localized values from the given dictionary.
        /// </summary>
        /// <param name="dictionary">The resource dictionary.</param>
        public static void Load(IResourceDictionary dictionary)
        {
            Requires.NotNull(dictionary, nameof(dictionary));

            var culture = dictionary.Culture;

            if (!dictionaries.ContainsKey(culture))
            {
                dictionaries[culture] = new AggregateDictionary(culture);
            }

            dictionaries[culture].Add(dictionary);
        }
    }
}