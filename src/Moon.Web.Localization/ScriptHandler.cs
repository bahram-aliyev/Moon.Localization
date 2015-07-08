using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Moon.Localization;
using Newtonsoft.Json;

namespace Moon.Web.Localization
{
    /// <summary>
    /// Writes localized values to HTTP response.
    /// </summary>
    public class ScriptHandler : IHttpHandler
    {
        /// <summary>
        /// Gets whether the handler can be reused by other requests.
        /// </summary>
        public bool IsReusable
            => true;

        /// <summary>
        /// Writes localized values to HTTP response.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/javascript";
            context.Response.StatusCode = 200;

            WriteScript(context.Response);
        }

        void WriteScript(HttpResponse response)
        {
            var script = new StringBuilder();

            script.Append("window.Resources={");
            script.Append($"cultures:{Serialize(GetCultures())},currentCulture:{Serialize(GetCurrentCulture())},");
            script.Append("get:function(category,name){category=(category||'').replace(new RegExp('/','g'),':');name=(name||'').replace(new RegExp('/','g'),':');");
            script.Append($"var values={Serialize(Resources.GetDictionary().Values)};");
            script.Append("var key=name.length>0?(category+':'+name):category;");
            script.Append("return values[key]||values[name];");
            script.Append("}};");

            response.Write(script.ToString());
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