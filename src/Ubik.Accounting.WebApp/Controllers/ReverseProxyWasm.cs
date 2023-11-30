using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ubik.Accounting.WebApp.ApiClients;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Webapp.Shared.Facades;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using MassTransit;


namespace Ubik.Accounting.WebApp.Controllers
{
    /// <summary>
    /// Works as a reverse proxy for interactive automode (wasm component)
    /// </summary>
    /// <param name="client"></param>
    [Authorize]
    [ApiController]
    public class ReverseProxyWasmController(IAccountingApiClient client) : ControllerBase
    {
        readonly IAccountingApiClient client = client;

        [HttpGet("/GetAllAccounts")]
        public async Task AccountList()
        {
            var response = await client.GetAllAccountsAsync();
            await ForwardResponse(response);
        }

        [HttpGet("/GetAccount/{id}")]
        public async Task Account(Guid id)
        {
            var response = await client.GetAccountAsync(id);
            await ForwardResponse(response);
        }

        private async Task ForwardResponse(HttpResponseMessage? responseMsg)
        {
            if (responseMsg == null) return;

            HttpContext.Response.StatusCode = (int)responseMsg.StatusCode;
            await responseMsg.Content.CopyToAsync(HttpContext.Response.Body);
        }
    }
}
