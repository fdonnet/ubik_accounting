using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ubik.Accounting.WebApp.ApiClients;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Webapp.Shared.Facades;
using System.Net.Http;
using Microsoft.AspNetCore.Http;


namespace Ubik.Accounting.WebApp.Controllers
{
    /// <summary>
    /// Works as a reverse proxy for interactive automode (wasm component)
    /// </summary>
    /// <param name="client"></param>
    [Authorize]
    [ApiController]
    public class TestController(IAccountingApiClient client) : ControllerBase
    {
        readonly IAccountingApiClient client = client;

        [HttpGet("/GetAllAccounts")]
        public async Task AccountList()
        {
            var responseMessage = await client.GetAllAccountsAsync();

            HttpContext.Response.StatusCode = (int)responseMessage.StatusCode;
            //CopyResponseHeaders(HttpContext, responseMessage);
            await responseMessage.Content.CopyToAsync(HttpContext.Response.Body);
        }
    }
}
