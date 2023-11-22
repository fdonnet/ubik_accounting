using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Webapp.Shared.Security
{
    public class CookieHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
