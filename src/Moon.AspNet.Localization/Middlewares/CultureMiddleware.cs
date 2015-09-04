using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Moon.Localization;

namespace Moon.AspNet.Localization
{
    /// <summary>
    /// Sets culture used by the application according to Accept-Language header.
    /// </summary>
    public class CultureMiddleware
    {
        const string headerName = "Accept-Language";
        const string cookieName = "Current-Culture";

        readonly RequestDelegate next;
        Action<DictionaryLoader> loader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureMiddleware" /> class
        /// </summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="loader">The dictionary loader.</param>
        public CultureMiddleware(RequestDelegate next, Action<DictionaryLoader> loader)
        {
            this.next = next;
            this.loader = loader;
        }

        /// <summary>
        /// Processes a requests and sets the current culture.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="host">The hosting environment.</param>
        public async Task Invoke(HttpContext context, IHostingEnvironment host)
        {
            LoadDictionaries(host);

            var culture = GetRequestedCulture(context);

            if (culture != null)
            {
                UpdateCurrentCulture(culture);
            }

            await next(context);
        }

        void LoadDictionaries(IHostingEnvironment host)
        {
            if (loader != null)
            {
                loader(new DictionaryLoader(host.WebRootPath));

                if (!host.IsDevelopment())
                {
                    loader = null;
                }
            }
        }

        CultureInfo GetRequestedCulture(HttpContext context)
        {
            var cookies = context.Request.Cookies;

            if (cookies.ContainsKey(cookieName))
            {
                return new CultureInfo(cookies[cookieName]);
            }

            var headers = context.Request.Headers;

            if (headers.ContainsKey(headerName))
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

#if DNXCORE50
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
#else
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
#endif
        }
    }
}