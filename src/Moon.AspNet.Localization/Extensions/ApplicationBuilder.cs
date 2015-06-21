using System;
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
            => app.UseLocalization(new PathString("/js/resources.js"), loader);

        /// <summary>
        /// Sets culture used by the application according to Accept-Language header.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="scriptPath">A path where to serialize a script with localized values.</param>
        /// <param name="loader">The dictionary loader.</param>
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app, PathString scriptPath, Action<DictionaryLoader> loader)
        {
            Requires.NotNull(scriptPath, nameof(scriptPath));
            Requires.NotNull(loader, nameof(loader));

            return app
                .UseMiddleware<CultureMiddleware>(loader)
                .Map(scriptPath, m =>
                {
                    m.UseMiddleware<ScriptMiddleware>();
                });
        }
    }
}