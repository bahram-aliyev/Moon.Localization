﻿using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Moon.Localization;

namespace Moon.AspNet.Localization.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLocalization(r => r
                .LoadJson("resources")
                .LoadXml("resources"));

            app.UseMvc();
        }
    }
}