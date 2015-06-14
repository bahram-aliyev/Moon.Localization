using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Moon.Localization.Tests
{
    public class AggregateDictionaryTests
    {
        readonly CultureInfo culture = new CultureInfo("en-US");

        [Fact]
        public void Add_ForDictionaryOfTheSameCulture_ValuesAreMerged()
        {
            var dictionary = new AggregateDictionary(culture);
            dictionary.Add(new FirstDictionary());

            Assert.Equal(2, dictionary.Values.Count);
            Assert.Equal("Value1", dictionary.Values["Key1"]);
            Assert.Equal("Value2", dictionary.Values["Key2"]);
        }

        [Fact]
        public void Add_ForDictionaryOfDifferentCulture_ExceptionIsThrown()
        {
            var dictionary = new AggregateDictionary(culture);

            Assert.Throws<CultureDoesNotMatchException>(() => dictionary.Add(new SecondDictionary()));
        }

        class FirstDictionary : IResourceDictionary
        {
            public CultureInfo Culture
                => new CultureInfo("en-US");

            public IDictionary<string, string> Values
                => new Dictionary<string, string>
                {
                    ["Key1"] = "Value1",
                    ["Key2"] = "Value2"
                };
        }

        class SecondDictionary : IResourceDictionary
        {
            public CultureInfo Culture
                => new CultureInfo("cs-CZ");

            public IDictionary<string, string> Values
                => new Dictionary<string, string>
                {
                    ["Key1"] = "Value1"
                };
        }
    }
}