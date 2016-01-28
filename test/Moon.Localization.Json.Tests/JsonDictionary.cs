using System;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Moon.Testing;
using Xunit;

namespace Moon.Localization.Json.Tests
{
    public class JsonDictionaryTests : TestSetup
    {
        FileStream stream;
        JsonDictionary dictionary;
        Action action;

        [Fact]
        public void LoadingJsonWithArrayAsRoot()
        {
            "Given the file"
                .x(() => stream = Use(File.OpenRead("Dictionaries/ArrayAsRoot.json")));

            "When I execute the Load action"
                .x(() => action = () => JsonDictionary.Load(stream));

            "Then it should throw an exception"
                .x(() =>
                {
                    action.ShouldThrow<FormatException>().WithMessage("Only an object can be the root. Path '', line 1 position 1.");
                });
        }

        [Fact]
        public void LoadingJsonWithUnexpectedEnd()
        {
            "Given the file"
                .x(() => stream = Use(File.OpenRead("Dictionaries/UnexpectedEnd.json")));

            "When I execute the Load action"
                .x(() => action = () => JsonDictionary.Load(stream));

            "Then it should throw an exception"
                .x(() =>
                {
                    action.ShouldThrow<FormatException>().WithMessage("Unexpected end when parsing JSON. Path 'Title', line 4 position 32.");
                });
        }

        [Fact]
        public void LoadingJsonWithUnknownToken()
        {
            "Given the file"
                .x(() => stream = Use(File.OpenRead("Dictionaries/UnknownToken.json")));

            "When I execute the Load action"
                .x(() => action = () => JsonDictionary.Load(stream));

            "Then it should throw an exception"
                .x(() =>
                {
                    action.ShouldThrow<FormatException>().WithMessage("Unsupported JSON token 'StartArray' was found. Path 'Search', line 6 position 15.");
                });
        }

        [Fact]
        public void LoadingDictionaryWithMissingCulture()
        {
            "Given the file"
                .x(() => stream = Use(File.OpenRead("Dictionaries/MissingCulture.json")));

            "When I execute the Load action"
                .x(() => action = () => JsonDictionary.Load(stream));

            "Then it should throw an exception"
                .x(() =>
                {
                    action.ShouldThrow<Exception>().WithMessage("The target culture could not be read form the dictionary.");
                });
        }

        [Fact]
        public void LoadingDictionary()
        {
            "Given the file"
                .x(() => stream = Use(File.OpenRead("Dictionaries/Correct.json")));

            "When I load the JSON dictionary"
                .x(() => dictionary = JsonDictionary.Load(stream));

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