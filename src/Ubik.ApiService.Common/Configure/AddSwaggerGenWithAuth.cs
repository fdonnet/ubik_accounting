using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Configure.Options;
using Ubik.ApiService.Common.Configure.Options.Swagger;

namespace Ubik.ApiService.Common.Configure
{
    public static class ServiceConfigurationSwaggerGenWithAuth
    {
        public static void AddSwaggerGenWithAuth(this IServiceCollection services, AuthServerOptions options, string xmlDocPath)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(
                                "oauth2",
                                new OpenApiSecurityScheme
                                {
                                    Type = SecuritySchemeType.OAuth2,
                                    Flows = new OpenApiOAuthFlows
                                    {
                                        AuthorizationCode = new OpenApiOAuthFlow
                                        {
                                            AuthorizationUrl = new Uri(options.AuthorizationUrl),
                                            TokenUrl = new Uri(options.TokenUrl),
                                            Scopes = new Dictionary<string, string> { }
                                        }
                                    }
                                });


                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "oauth2", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                            new List<string>()
                    }
                    });

                c.OperationFilter<SwaggerDefaultValues>();
                c.IncludeXmlComments(xmlDocPath);
            });
        }
    }
}
