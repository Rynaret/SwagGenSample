using App.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
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
            services.AddMvc();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://demo.identityserver.io/";
                    options.TokenValidationParameters.ValidateAudience = false;
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://demo.identityserver.io/";

                    options.ClientId = "interactive.confidential.short";
                    options.ClientSecret = "secret";
                    options.SaveTokens = true;
                    options.ResponseType = "code";
                    options.UseTokenLifetime = true;

                    options.Scope.Add("offline_access");
                });

            // Add OpenAPI/Swagger document
            services.AddOpenApiDocument(configure =>
            {
                configure.SchemaNameGenerator = new CustomSchemaNameGenerator();
                configure.TypeNameGenerator = new CustomTypeNameGenerator();
                configure.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "App API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "Renat Sungatullin",
                        Email = string.Empty,
                        Url = "https://medium.com/dev-genius/csharp-protecting-swagger-endpoints-82ae5cfc7eb1"
                    };
                };

                configure.AddSecurity("bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        AuthorizationCode = new OpenApiOAuthFlow()
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                { "api", "" },
                            },
                            AuthorizationUrl = "https://demo.identityserver.io/connect/authorize",
                            TokenUrl = "https://demo.identityserver.io/connect/token"
                        },
                    }
                });

                configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Add OpenAPI/Swagger middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3(ConfigureSwaggerUi);

            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapControllers()
                    .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme });

                var pipeline = endpoints.CreateApplicationBuilder().Build();
                var oidcAuthAttr = new AuthorizeAttribute { AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme };
                endpoints
                    .Map("/swagger/{documentName}/swagger.json", pipeline)
                    .RequireAuthorization(oidcAuthAttr);
                endpoints
                    .Map("/swagger/index.html", pipeline)
                    .RequireAuthorization(oidcAuthAttr);
            });
        }

        private static Action<SwaggerUi3Settings> ConfigureSwaggerUi =>
            settings =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "interactive.confidential.short",
                    ClientSecret = "secret",
                    AppName = "Weather",
                    UsePkceWithAuthorizationCodeGrant = true
                };
                settings.AdditionalSettings.Add("requestInterceptor: swagOidc.requestInterceptor,//", "");
                settings.AdditionalSettings.Add("showMutatedRequest", false);
                settings.CustomJavaScriptPath = "index.js";
            };
    }
}
