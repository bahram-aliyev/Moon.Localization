using System.Collections.Generic;
using System.IO;
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
            var values = Resources.GetDictionary().Values;

            script.Append("window.Resources={get:function(category,name){");
            script.Append("category=(category||'').replace(new RegExp('/','g'),':');name=(name||'').replace(new RegExp('/','g'),':');");
            script.Append($"var values={Serialize(values)};");
            script.Append("var key=name.length>0?(category+':'+name):category;");
            script.Append("return values[key]||values[name];");
            script.Append("}};");

            response.Write(script.ToString());
        }

        string Serialize(IDictionary<string, string> values)
        {
            using (var writer = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.QuoteName = true;

                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, values);

                return writer.ToString();
            }
        }
    }
}