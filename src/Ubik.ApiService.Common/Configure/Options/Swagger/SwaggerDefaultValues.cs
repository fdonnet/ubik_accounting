﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace Ubik.ApiService.Common.Configure.Options.Swagger
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        //TODO: change that by a config option
        //When false the header x-user-id and x-tenant-id are not showed in Swagger but needed (the proxy take care of THAT)
        //When true the header x-user-id and x-tenant-id is showed (can be call internally with direct values, via postman or other services)
        private static bool ShowHeaderParam = false;
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            operation.Deprecated |= apiDescription.IsDeprecated();

            foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
            {
                var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
                var response = operation.Responses[responseKey];

                foreach (var contentType in response.Content.Keys)
                {
                    if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
                    {
                        response.Content.Remove(contentType);
                    }
                }
            }

            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                parameter.Description ??= description.ModelMetadata?.Description;

                if (parameter.Schema.Default == null &&
                     description.DefaultValue != null &&
                     description.DefaultValue is not DBNull &&
                     description.ModelMetadata is ModelMetadata modelMetadata)
                {
                    // REF: https://github.com/Microsoft/aspnet-api-versioning/issues/429#issuecomment-605402330
                    var json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                    parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
                }

                parameter.Required |= description.IsRequired;
            }

            //Add header param
            if(ShowHeaderParam)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "x-user-id",
                    In = ParameterLocation.Header,
                    Required = false
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "x-tenant-id",
                    In = ParameterLocation.Header,
                    Required = false
                });
            }
        }
    }
}
