using System;
using System.IO;
using CommerceServer.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CommerceServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        private const string CorsConfiguration = "CorsConfiguration";

        public void ConfigureServices(IServiceCollection services)
        {
            var sqlitePath = Path.Combine(Environment.CurrentDirectory, @"DAL\commerce.db");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={sqlitePath}")
            );
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsConfiguration, builder =>
                {
                    builder.WithOrigins("http://localhost:3000");
                });
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            app.UseRouting();
            app.UseCors(CorsConfiguration);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}