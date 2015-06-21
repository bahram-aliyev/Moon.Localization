using System.Collections.Generic;
using System.IO;
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
            return WriteScript(context.Response);
        }

        Task WriteScript(IOwinResponse response)
        {
            var script = new StringBuilder();
            var values = Resources.GetDictionary().Values;

            script.Append("window.Resources={get:function(category,name){");
            script.Append("category=(category||'').replace(new RegExp('/','g'),':');name=(name||'').replace(new RegExp('/','g'),':');");
            script.Append($"var values={Serialize(values)};");
            script.Append("var key=name.length>0?(category+':'+name):category;");
            script.Append("return values[key]||values[name];");
            script.Append("}};");

            return response.WriteAsync(script.ToString());
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