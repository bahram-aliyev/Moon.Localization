using System;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Moon.Testing;
using Xunit;

namespace Moon.Localization.Xml.Tests
{
    public class XmlDictionaryTests : TestSetup
    {
        FileStream stream;
        XmlDictionary dictionary;
        Action action;

        [Fact]
        public void LoadDictionaryWithMissingCulture()
        {
            "Given the file"
                .x(() => stream = Use(File.OpenRead("Dictionaries/MissingCulture.xml")));

            "When I execute the Load action"
                .x(() => action = () => XmlDictionary.Load(stream));

            "Then it should throw an exception"
                .x(() =>
                {
                    action.ShouldThrow<Exception>().WithMessage("The target culture could not be read form the dictionary.");
                });
        }

        [Fact]
        public void LoadDictionary()
        {
            "Given the file"
                .x(() => stream = Use(File.OpenRead("Dictionaries/Correct.xml")));

            "When I load the JSON dictionary"
                .x(() => dictionary = XmlDictionary.Load(stream));

            "Then it should be loaded correctly"
                .x(() =>
                {
                    dictionary.Culture.Should().Be(new CultureInfo("en"));
                    dictionary.Values["Title"].Should().Be("Application Title");
                    dictionary.Values["Search:NotSupported"].Should().Be("Not supported");
                    dictionary.Values["Search:Label"].Should().Be("Search");
                });
        }
    }
}