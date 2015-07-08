using System.Collections;
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

            script.Append("window.Resources={");
            script.Append($"cultures:{Json.Serialize(GetCultures(), true)},currentCulture:{Json.Serialize(GetCurrentCulture(), true)},");
            script.Append("get:function(category,name){category=(category||'').replace(new RegExp('/','g'),':');name=(name||'').replace(new RegExp('/','g'),':');");
            script.Append($"var values={Json.Serialize(Resources.GetDictionary().Values, true)};");
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
    }
}