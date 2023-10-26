using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Configure.Options
{
    public class SwaggerUIOptions : IOptionsPosition
    {
        public const string Position = "SwaggerUI";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
