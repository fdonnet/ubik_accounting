using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Ubik.ApiService.Common.Configure
{
    public static class ServiceConfigurationApiVersion
    {
        public static void AddApiVersionAndExplorer(this IServiceCollection services)
        {
            var apiVersionBuilder = services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = false;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
            });

            apiVersionBuilder.AddApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });
        }
    }
}
