using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using Moon.Localization;

namespace Moon.Web.Localization
{
    /// <summary>
    /// Sets culture used by the application according to Accept-Language header.
    /// </summary>
    public class CultureModule : IHttpModule
    {
        const string headerName = "Accept-Language";
        const string cookieName = "Current-Culture";

        /// <summary>
        /// </summary>
        /// <param name="app">The HTTP application.</param>
        public void Init(HttpApplication app)
        {
            app.BeginRequest += BeginRequest;
        }

        /// <summary>
        /// Frees resources used by the HTTP module.
        /// </summary>
        public void Dispose()
        {
            // NOOP
        }

        void BeginRequest(object sender, EventArgs args)
        {
            var context = (sender as HttpApplication).Context;
            var culture = GetRequestedCulture(context);

            if (culture != null)
            {
                UpdateCurrentCulture(culture);
            }
        }

        CultureInfo GetRequestedCulture(HttpContext context)
        {
            var cookies = context.Request.Cookies;

            if (cookies[cookieName] != null)
            {
                return new CultureInfo(cookies[cookieName].Value);
            }

            var headers = context.Request.Headers;

            if (headers.AllKeys.Contains(headerName))
            {
                var header = headers[headerName];
                var cultureName = header.Split(' ', ',', ';')[0];
                return new CultureInfo(cultureName);
            }

            return null;
        }

        void UpdateCurrentCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}