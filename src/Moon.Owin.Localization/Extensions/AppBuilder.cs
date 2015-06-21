﻿using System;
using Microsoft.Owin;
using Moon;
using Moon.Localization;
using Moon.Owin.Localization;

namespace Owin
{
    /// <summary>
    /// <see cref="IAppBuilder" /> extension methods.
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Sets culture used by the application according to Accept-Language header.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="loader">The dictionary loader.</param>
        public static IAppBuilder UseLocalization(this IAppBuilder app, Action<DictionaryLoader> loader)
            => app.UseLocalization(new PathString("/js/resources.js"), loader);

        /// <summary>
        /// Sets culture used by the application according to Accept-Language header.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="scriptPath">A path where to serialize a script with localized values.</param>
        /// <param name="loader">The dictionary loader.</param>
        public static IAppBuilder UseLocalization(this IAppBuilder app, PathString scriptPath, Action<DictionaryLoader> loader)
        {
            Requires.NotNull(scriptPath, nameof(scriptPath));
            Requires.NotNull(loader, nameof(loader));

            return app
                .Use<CultureMiddleware>(loader)
                .Map(scriptPath, m =>
                {
                    m.Use<ScriptMiddleware>();
                });
        }
    }
}