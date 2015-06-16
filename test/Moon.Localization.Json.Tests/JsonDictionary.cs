using System;
using System.Globalization;
using System.IO;
using Xunit;

namespace Moon.Localization.Json.Tests
{
    public class JsonDictionaryTests
    {
        [Fact]
        public void Load_ForJsonWithArrayAsRoot_ThrowsException()
        {
            using (var stream = File.OpenRead("Dictionaries/ArrayAsRoot.json"))
            {
                var exception = Assert.Throws<FormatException>(() => JsonDictionary.Load(stream));
                Assert.StartsWith("Only an object can be the root.", exception.Message);
            }
        }

        [Fact]
        public void Load_ForJsonWithUnexpectedEnd_ThrowsException()
        {
            using (var stream = File.OpenRead("Dictionaries/UnexpectedEnd.json"))
            {
                var exception = Assert.Throws<FormatException>(() => JsonDictionary.Load(stream));
                Assert.StartsWith("Unexpected end when parsing JSON.", exception.Message);
            }
        }

        [Fact]
        public void Load_ForJsonWithUnknownToken_ThrowsException()
        {
            using (var stream = File.OpenRead("Dictionaries/UnknownToken.json"))
            {
                var exception = Assert.Throws<FormatException>(() => JsonDictionary.Load(stream));
                Assert.StartsWith("Unsupported JSON token", exception.Message);
            }
        }

        [Fact]
        public void Load_ForDictionaryWithMissingCulture_ThrowsException()
        {
            using (var stream = File.OpenRead("Dictionaries/MissingCulture.json"))
            {
                var exception = Assert.Throws<Exception>(() => JsonDictionary.Load(stream));
                Assert.Equal("The target culture could not be read form the dictionary.", exception.Message);
            }
        }

        [Fact]
        public void Load_ForCorrectDictionary_LoadsValues()
        {
            using (var stream = File.OpenRead("Dictionaries/Correct.json"))
            {
                var culture = new CultureInfo("en");
                var dictionary = JsonDictionary.Load(stream);

                Assert.Equal(culture, dictionary.Culture);
                Assert.Equal("Application Title", dictionary.Values["Title"]);
                Assert.Equal("Not supported", dictionary.Values["Search:NotSupported"]);
                Assert.Equal("Search", dictionary.Values["Search:Label"]);
            }
        }
    }
}