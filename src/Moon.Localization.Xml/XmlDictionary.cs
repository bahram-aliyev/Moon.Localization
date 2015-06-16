using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace Moon.Localization.Xml
{
    /// <summary>
    /// The language definition loaded from XML files.
    /// </summary>
    public class XmlDictionary : IResourceDictionary
    {
        static readonly XNamespace xmlNamespace = "http://xml.moontea.net/localization";

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
        public static XmlDictionary Load(Stream data)
        {
            Requires.NotNull(data, nameof(data));

            var dictionary = new XmlDictionary();

            using (var reader = new StreamReader(data))
            {
                var document = XDocument.Load(reader);

                dictionary.Culture = GetCulture(document.Root);
                FillValues(dictionary, document.Root);
            }

            if (dictionary.Culture == null)
            {
                throw new Exception("The target culture could not be read form the dictionary.");
            }

            return dictionary;
        }

        static CultureInfo GetCulture(XElement element)
        {
            var attribute = element.Attribute("culture");

            if (attribute != null)
            {
                return new CultureInfo(attribute.Value);
            }

            return null;
        }

        static void FillValues(XmlDictionary dictionary, XElement element, string parentKey = null)
        {
            foreach (var child in element.Elements(xmlNamespace + "category"))
            {
                FillValues(dictionary, child, GetKey(parentKey, child));
            }

            foreach (var child in element.Elements(xmlNamespace + "localized"))
            {
                dictionary.Values[GetKey(parentKey, child)] = GetValue(child);
            }
        }

        static string GetKey(string parentKey, XElement element)
        {
            var attribute = element.Attribute("name");

            if (attribute == null)
            {
                throw new Exception("The value name / key is missing.");
            }

            if(parentKey != null)
            {
                return $"{parentKey}:{attribute.Value}";
            }

            return attribute.Value;
        }

        static string GetValue(XElement element)
        {
            var child = element.Element(xmlNamespace + "value");

            if (child == null)
            {
                throw new Exception("The value is missing.");
            }

            return child.Value;
        }
    }
}