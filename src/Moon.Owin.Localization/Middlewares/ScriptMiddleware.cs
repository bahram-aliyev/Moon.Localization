using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Moon.Localization;
using Newtonsoft.Json;

namespace Moon.Owin.Localization
{
    /// <summary>
    /// Writes localized values to HTTP response.
    /// </summary>
    public class ScriptMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptMiddleware" /> class
        /// </summary>
        /// <param name="next">The next middleware.</param>
        public ScriptMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        /// <summary>
        /// Writes localized values to HTTP response.
        /// </summary>
        /// <param name="context">The OWIN context.</param>
        public override Task Invoke(IOwinContext context)
        {
            context.Response.ContentType = "text/javascript";
            context.Response.StatusCode = 200;

            return WriteScript(context.Response);
        }

        Task WriteScript(IOwinResponse response)
        {
            var script = new StringBuilder();

            script.Append("window.Resources={");
            script.Append($"cultures:{Serialize(GetCultures())},currentCulture:{Serialize(GetCurrentCulture())},");
            script.Append("get:function(category,name){category=(category||'').replace(new RegExp('/','g'),':');name=(name||'').replace(new RegExp('/','g'),':');");
            script.Append($"var values={Serialize(Resources.GetDictionary().Values)};");
            script.Append("var key=name.length>0?(category+':'+name):category;");
            script.Append("return values[key]||values[name];");
            script.Append("}};");

            return response.WriteAsync(script.ToString());
        }

        IEnumerable GetCultures()
        {
            return Resources.Cultures.Select(c => new
            {
                name = c.Name,
                isoName = c.TwoLetterISOLanguageName,
                nativeName = c.NativeName
            });
        }

        IEnumerable<string> GetCurrentCulture()
        {
            var culture = Resources.CurrentCulture;

            yield return culture.Name;

            if (culture.Parent != null && culture.Parent.Name.Length > 0)
            {
                yield return culture.Parent.Name;
            }
        }

        string Serialize(object obj)
        {
            using (var writer = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.QuoteName = true;

                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, obj);

                return writer.ToString();
            }
        }
    }
}