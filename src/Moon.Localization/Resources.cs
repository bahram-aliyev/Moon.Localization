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
        /// Gets or sets the default culture used when a dictionary for the
        /// <see cref="CultureInfo.CurrentUICulture" /> or its <see cref="CultureInfo.Parent" />
        /// does not exist.
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
        /// Gets the culture of the dictionary we will load resources from. It's equal to either
        /// <see cref="CultureInfo.CurrentUICulture" />, <see cref="DefaultCulture" /> or one of
        /// their <see cref="CultureInfo.Parent" /> cultures.
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                var culture = CultureInfo.CurrentUICulture;

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
                    culture = culture.Parent;
                }

                return culture;
            }
        }

        /// <summary>
        /// Gets an enumeration of all cultures dictionaries are available for.
        /// </summary>
        public static IEnumerable<CultureInfo> Cultures
            => dictionaries.Keys;

        /// <summary>
        /// Returns a resource with the given name; or <c>null</c> if the resource does not exist.
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
        /// <param name="category">The name of the category.</param>
        /// <param name="name">The name of the resource.</param>
        public static string Get(string category, string name)
        {
            Requires.NotNullOrWhiteSpace(category, nameof(category));
            Requires.NotNullOrWhiteSpace(name, nameof(name));

            return Get($"{category}:{name}") ?? Get(name);
        }

        /// <summary>
        /// Returns a resource dictionary for the current culture.
        /// </summary>
        public static IResourceDictionary GetDictionary()
        {
            if (!dictionaries.ContainsKey(CurrentCulture))
            {
                throw new DictionaryNotFoundException();
            }

            return dictionaries[CurrentCulture];
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