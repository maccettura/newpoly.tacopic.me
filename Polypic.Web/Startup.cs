using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Polypic.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //full

                routes.MapRoute(
                    name: "gif-full",
                    template: "{width}/{height}/{color1}/{color2}/{steps}.gif",
                    defaults: new { controller = "Home", action = "Index", source = "gif" });

                routes.MapRoute(
                    name: "jpg-full",
                    template: "{width}/{height}/{color1}/{color2}/{steps}.jpg",
                    defaults: new { controller="Home", action="Index", source = "jpg"});

                routes.MapRoute(
                    name: "jpeg-full",
                    template: "{width}/{height}/{color1}/{color2}/{steps}.jpeg",
                    defaults: new { controller = "Home", action = "Index", source = "jpeg" });

                routes.MapRoute(
                    name: "png-full",
                    template: "{width}/{height}/{color1}/{color2}/{steps}.png",
                    defaults: new { controller = "Home", action = "Index", source = "png" });

                //partial

                routes.MapRoute(
                    name: "gif-partial",
                    template: "{width}/{height}/{steps=5}.gif",
                    defaults: new { controller = "Home", action = "Index", source = "gif" });

                routes.MapRoute(
                    name: "jpg-partial",
                    template: "{width}/{height}/{steps=5}.jpg",
                    defaults: new { controller = "Home", action = "Index", source = "jpg" });

                routes.MapRoute(
                    name: "jpeg-partial",
                    template: "{width}/{height}/{steps=5}.jpeg",
                    defaults: new { controller = "Home", action = "Index", source = "jpeg" });

                routes.MapRoute(
                    name: "png-partial",
                    template: "{width}/{height}/{steps=5}.png",
                    defaults: new { controller = "Home", action = "Index", source = "png" });

                routes.MapRoute(
                    name: "partial-default",
                    template: "{width}/{height}/{steps=5}",
                    defaults: new { controller = "Home", action = "Index", source = "jpg" });

                //default
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}",
                    defaults:  new { width=400, height=400, steps=5, source="jpg"});
            });
        }
    }
}
