using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Moon.Localization.Json
{
    /// <summary>
    /// The language definition loaded from XML files.
    /// </summary>
    public class JsonDictionary : IResourceDictionary
    {
        static readonly StringComparison comparison = StringComparison.Ordinal;

        /// <summary>
        /// Gets the target culture.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Gets a collection of localized values.
        /// </summary>
        public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Loads resource dictionary from the given stream.
        /// </summary>
        /// <param name="data">The stream containing XML resource dictionary.</param>
        public static JsonDictionary Load(Stream data)
        {
            Requires.NotNull(data, nameof(data));

            var dictionary = new JsonDictionary();

            using (var reader = new JsonTextReader(new StreamReader(data)))
            {
                var startCount = 0;
                reader.DateParseHandling = DateParseHandling.None;
                reader.Read();

                SkipComments(reader);

                if (reader.TokenType != JsonToken.StartObject)
                {
                    throw new FormatException($"Only an object can be the root. Path '{reader.Path}', line {reader.LineNumber} position {reader.LinePosition}.");
                }

                do
                {
                    SkipComments(reader);

                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject:
                            startCount++;
                            break;

                        case JsonToken.EndObject:
                            startCount--;
                            break;

                        case JsonToken.PropertyName:
                            break;

                        case JsonToken.String:
                            var key = GetKey(reader.Path);
                            var value = reader.Value.ToString();

                            if (key == "culture")
                            {
                                dictionary.Culture = new CultureInfo(value);
                            }
                            else
                            {
                                dictionary.Values[key] = value;
                            }
                            break;

                        case JsonToken.None:
                            throw new FormatException($"Unexpected end when parsing JSON. Path '{reader.Path}', line {reader.LineNumber} position {reader.LinePosition}.");

                        default:
                            throw new FormatException($"Unsupported JSON token '{reader.TokenType}' was found. Path '{reader.Path}', line {reader.LineNumber} position {reader.LinePosition}.");
                    }

                    reader.Read();
                }
                while (startCount > 0);
            }

            if (dictionary.Culture == null)
            {
                throw new Exception("The target culture could not be read form the dictionary.");
            }

            return dictionary;
        }

        static string GetKey(string jsonPath)
        {
            var index = 0;
            var pathSegments = new List<string>();

            while (index < jsonPath.Length)
            {
                var start = jsonPath.IndexOf("['", index, comparison);

                if (start < 0)
                {
                    pathSegments.Add(jsonPath.Substring(index).Replace('.', ':'));
                    break;
                }
                else
                {
                    if (start > index)
                    {
                        pathSegments.Add(jsonPath.Substring(index, start - index).Replace('.', ':'));
                    }

                    var endIndex = jsonPath.IndexOf("']", start, comparison);
                    pathSegments.Add(jsonPath.Substring(start + 2, endIndex - start - 2));
                    index = endIndex + 2;
                }
            }

            return string.Join(string.Empty, pathSegments);
        }

        static void SkipComments(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }
        }
    }
}