using System;
using System.Globalization;
using System.IO;
using Xunit;

namespace Moon.Localization.Xml.Tests
{
    public class XmlDictionaryTests
    {
        [Fact]
        public void Load_ForDictionaryWithMissingCulture_ThrowsException()
        {
            using (var stream = File.OpenRead("Dictionaries/MissingCulture.xml"))
            {
                var exception = Assert.Throws<Exception>(() => XmlDictionary.Load(stream));
                Assert.Equal("The target culture could not be read form the dictionary.", exception.Message);
            }
        }

        [Fact]
        public void Load_ForCorrectDictionary_LoadsValues()
        {
            using (var stream = File.OpenRead("Dictionaries/Correct.xml"))
            {
                var culture = new CultureInfo("en");
                var dictionary = XmlDictionary.Load(stream);

                Assert.Equal(culture, dictionary.Culture);
                Assert.Equal("Application Title", dictionary.Values["Title"]);
                Assert.Equal("Not supported", dictionary.Values["Search:NotSupported"]);
                Assert.Equal("Search", dictionary.Values["Search:Label"]);
            }
        }
    }
}