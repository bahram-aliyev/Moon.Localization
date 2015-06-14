using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace Moon.Localization
{
    /// <summary>
    /// The resource dictionary used to aggregate several dictionaries for the same culture.
    /// Categories and values are merged. Values with existing names are replaced.
    /// </summary>
    public class AggregateDictionary : IResourceDictionary
    {
        readonly StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
        readonly ConcurrentDictionary<string, string> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateDictionary" /> class.
        /// </summary>
        /// <param name="culture">The target culture.</param>
        public AggregateDictionary(CultureInfo culture)
        {
            Requires.NotNull(culture, nameof(culture));

            Culture = culture;
            values = new ConcurrentDictionary<string, string>(comparer);
        }

        /// <summary>
        /// Gets the target culture.
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets a collection of localized values.
        /// </summary>
        public IDictionary<string, string> Values
            => values;

        /// <summary>
        /// Adds another resource dictionary. Categories and values are merged. Values with existing
        /// name are replaced.
        /// </summary>
        /// <param name="dictionary">The resource dictionary.</param>
        /// <exception cref="CultureDoesNotMatchException">
        /// Added dictionary is for different culture than the current dictionary.
        /// </exception>
        public void Add(IResourceDictionary dictionary)
        {
            Requires.NotNull(dictionary, nameof(dictionary));

            if (!Equals(Culture, dictionary.Culture))
            {
                throw new CultureDoesNotMatchException();
            }

            MergeValues(values, dictionary.Values);
        }

        void MergeValues(IDictionary<string, string> currentValues, IDictionary<string, string> addedValues)
        {
            foreach (var item in addedValues)
            {
                currentValues[item.Key] = item.Value;
            }
        }
    }
}