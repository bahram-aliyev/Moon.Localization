﻿using System;
using System.Globalization;
using Microsoft.AspNet.Http;
using Moon;
using Moon.AspNet.Localization;
using Moon.Localization;

namespace Microsoft.AspNet.Builder
{
    /// <summary>
    /// <see cref="IApplicationBuilder" /> extension methods.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Sets culture used by the application according to Accept-Language header.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="loader">The dictionary loader.</param>
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app, Action<DictionaryLoader> loader)
            => app.UseLocalization("en-US", loader);

        /// <summary>
        /// Sets culture used by the application according to Accept-Language header.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="defaultCulture">
        /// The default culture used when a dictionary for the current culture does not exist.
        /// </param>
        /// <param name="loader">The dictionary loader.</param>
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app, string defaultCulture, Action<DictionaryLoader> loader)
            => app.UseLocalization(defaultCulture, new PathString("/assets/resources.js"), loader);

        /// <summary>
        /// Sets culture used by the application according to Accept-Language header.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="scriptPath">A path where to serialize a script with localized values.</param>
        /// <param name="loader">The dictionary loader.</param>
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app, PathString scriptPath, Action<DictionaryLoader> loader)
            => app.UseLocalization("en-US", scriptPath, loader);

        /// <summary>
        /// Sets culture used by the application according to Accept-Language header.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="defaultCulture">
        /// The default culture used when a dictionary for the current culture does not exist.
        /// </param>
        /// <param name="scriptPath">A path where to serialize a script with localized values.</param>
        /// <param name="loader">The dictionary loader.</param>
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app, string defaultCulture, PathString scriptPath, Action<DictionaryLoader> loader)
        {
            Requires.NotNull(defaultCulture, nameof(defaultCulture));
            Requires.NotNull(scriptPath, nameof(scriptPath));
            Requires.NotNull(loader, nameof(loader));

            Resources.DefaultCulture = new CultureInfo(defaultCulture);

            return app
                .UseMiddleware<CultureMiddleware>(loader)
                .Map(scriptPath, m =>
                {
                    m.UseMiddleware<ScriptMiddleware>();
                });
        }
    }
}