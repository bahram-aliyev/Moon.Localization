using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Moon.Localization;

namespace Moon.AspNet.Localization
{
    /// <summary>
    /// Writes localized values to HTTP response.
    /// </summary>
    public class ScriptMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptMiddleware" /> class
        /// </summary>
        /// <param name="next">The next middleware.</param>
        public ScriptMiddleware(RequestDelegate next)
        {
        }

        /// <summary>
        /// Writes localized values to HTTP response.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        public Task Invoke(HttpContext context)
        {
            context.Response.ContentType = "text/javascript";
            context.Response.StatusCode = 200;

            return WriteScript(context.Response);
        }

        Task WriteScript(HttpResponse response)
        {
            var script = new StringBuilder();
            var cultures = Resources.Cultures.Select(c => c.Name);
            var values = Resources.GetDictionary().Values;

            script.Append("window.Resources={");
            script.Append($"cultures:{Json.Serialize(cultures)},currentCulture:{Json.Serialize(GetCurrentCultures())},");
            script.Append("get:function(category,name){category=(category||'').replace(new RegExp('/','g'),':');name=(name||'').replace(new RegExp('/','g'),':');");
            script.Append($"var values={Json.Serialize(values, true)};");
            script.Append("var key=name.length>0?(category+':'+name):category;");
            script.Append("return values[key]||values[name];");
            script.Append("}};");

            return response.WriteAsync(script.ToString());
        }

        IEnumerable<string> GetCurrentCultures()
        {
            var culture = Resources.CurrentCulture;

            yield return culture.Name;

            if (culture.Parent != null && culture.Parent.Name.Length > 0)
            {
                yield return culture.Parent.Name;
            }
        }
    }
}