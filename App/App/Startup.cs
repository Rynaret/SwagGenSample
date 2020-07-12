using App.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.AspNetCore;
using System.Linq;

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
            services.AddSingleton<VersionTaggingOperationProcessor>();
            services
                .AddOpenApiDocument(configure =>
                {
                    BaseConfigure(configure, "v1");
                })
                .AddOpenApiDocument(configure =>
                {
                    BaseConfigure(configure, "v2");
                })
                .AddOpenApiDocument((configure, serviceProvider) =>
                {
                    BaseConfigure(configure, "vall", new[] { "v1", "v2" });
                    configure.OperationProcessors.Add(serviceProvider.GetService<VersionTaggingOperationProcessor>());
                });
        }

        private void BaseConfigure(AspNetCoreOpenApiDocumentGeneratorSettings configure, string docName, string[] apiGroupNames)
        {
            configure.SchemaNameGenerator = new CustomSchemaNameGenerator();
            configure.TypeNameGenerator = new CustomTypeNameGenerator();

            configure.DocumentName = docName;
            configure.ApiGroupNames = apiGroupNames;

            configure.PostProcess = document =>
            {
                document.Info.Version = docName;
                document.Info.Title = "App API";
                document.Info.Description = "A simple ASP.NET Core web API";
                document.Info.TermsOfService = "None";
                document.Info.Contact = new OpenApiContact
                {
                    Name = "Renat Sungatullin",
                    Email = string.Empty,
                    Url = "some medium"
                };
                document.Tags = apiGroupNames.Select(x => new OpenApiTag { Name = x }).ToList();
            };
        }
        private void BaseConfigure(AspNetCoreOpenApiDocumentGeneratorSettings configure, string docName)
        {
            BaseConfigure(configure, docName, new[] { docName });
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
