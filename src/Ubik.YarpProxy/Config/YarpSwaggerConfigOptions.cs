using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Ubik.ApiService.Common.Configure.Options.Swagger;
using Ubik.YarpProxy.Extensions;
using Yarp.ReverseProxy.Swagger;

namespace Ubik.YarpProxy.Config
{
    public class YarpSwaggerConfigOptions(
    IOptionsMonitor<ReverseProxyDocumentFilterConfig> reverseProxyDocumentFilterConfigOptions)
    : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly ReverseProxyDocumentFilterConfig _reverseProxyDocumentFilterConfig = reverseProxyDocumentFilterConfigOptions.CurrentValue;

        public void Configure(SwaggerGenOptions options)
        {
            var filterDescriptors = new List<FilterDescriptor>();

            options.ConfigureSwaggerDocs(_reverseProxyDocumentFilterConfig);

            filterDescriptors.Add(new FilterDescriptor
            {
                Type = typeof(ReverseProxyDocumentFilter),
                Arguments = []
            });

            options.DocumentFilterDescriptors = filterDescriptors;
        }
    }
}
