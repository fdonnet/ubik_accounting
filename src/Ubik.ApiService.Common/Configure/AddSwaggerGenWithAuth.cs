﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
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
                                        //ClientCredentials = new OpenApiOAuthFlow
                                        //{
                                        //    AuthorizationUrl = new Uri(options.TokenUrl),
                                        //    TokenUrl = new Uri(options.TokenUrl)
                                        //}
                                    },
                                    // OpenIdConnectUrl = new Uri(options.MetadataAddress)
                                }) ;
                //"OpenIdConnect",
                //new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.OpenIdConnect,
                //    OpenIdConnectUrl = new Uri(options.MetadataAddress)
                //}
                //);


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
