using App.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.AspNetCore;
using System;

namespace App
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
            services
                .AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                })
                .AddMvc();

            services
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            // Add OpenAPI/Swagger documents
            services
                .AddOpenApiDocument(configure =>
                {
                    BaseConfigure(configure, "v1");
                })
                .AddOpenApiDocument(configure =>
                {
                    BaseConfigure(configure, "v2");
                });
        }

        private void BaseConfigure(AspNetCoreOpenApiDocumentGeneratorSettings configure, string version)
        {
            configure.SchemaNameGenerator = new CustomSchemaNameGenerator();
            configure.TypeNameGenerator = new CustomTypeNameGenerator();

            configure.DocumentName = version;
            configure.ApiGroupNames = new[] { version };

            configure.PostProcess = document =>
            {
                document.Info.Version = version;
                document.Info.Title = "App API";
                document.Info.Description = "A simple ASP.NET Core web API";
                document.Info.TermsOfService = "None";
                document.Info.Contact = new OpenApiContact
                {
                    Name = "Renat Sungatullin",
                    Email = string.Empty,
                    Url = "some medium"
                };
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add OpenAPI/Swagger middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
