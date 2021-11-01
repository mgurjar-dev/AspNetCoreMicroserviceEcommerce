using Catelog.API.Controllers;
using Catelog.API.Data;
using Catelog.API.Repository;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Catelog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catelog.API", Version = "v1" });
            });
            services.AddScoped<ICatelogContext, CatelogContext>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddOpenTelemetryTracing(config => config
               .AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               
               //.AddSource(nameof(CatelogController))
               //.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CatalogTelemetry"))
               .AddMongoDBInstrumentation()
               .AddConsoleExporter()
               .AddZipkinExporter(o =>
               {
                   o.Endpoint = new Uri(Configuration["ApiSettings:ZipkinUrl"]);

               })
               );

            services.AddHealthChecks().AddMongoDb(
                            Configuration["DatabaseSettings:ConnectionString"],
                            "Catalog MongoDb Health",
                            HealthStatus.Degraded);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;
            logger.LogInformation("Starting catelog api");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catelog.API v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}
