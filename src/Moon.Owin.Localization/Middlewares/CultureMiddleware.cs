﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Moon.Localization;

namespace Moon.Owin.Localization
{
    /// <summary>
    /// Sets culture used by the application according to Accept-Language header.
    /// </summary>
    public class CultureMiddleware : OwinMiddleware
    {
        const string headerName = "Accept-Language";
        const string cookieName = "Current-Culture";

        readonly OwinMiddleware next;
        readonly Action<DictionaryLoader> loader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureMiddleware" /> class
        /// </summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="loader">The dictionary loader.</param>
        public CultureMiddleware(OwinMiddleware next, Action<DictionaryLoader> loader)
            : base(next)
        {
            this.next = next;
            this.loader = loader;
        }

        /// <summary>
        /// Processes a requests and sets the current culture.
        /// </summary>
        /// <param name="context">The OWIN context.</param>
        public override async Task Invoke(IOwinContext context)
        {
            loader(new DictionaryLoader(GetWebRootPath()));

            var culture = GetRequestedCulture(context);

            if (culture != null)
            {
                UpdateCurrentCulture(culture);
            }

            await next.Invoke(context);
        }

        string GetWebRootPath()
            => AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        CultureInfo GetRequestedCulture(IOwinContext context)
        {
            var cookies = context.Request.Cookies;

            if (cookies[cookieName] != null)
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

            return Resources.DefaultCulture;
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