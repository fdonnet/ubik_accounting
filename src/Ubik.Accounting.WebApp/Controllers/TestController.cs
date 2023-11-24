using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ubik.Accounting.WebApp.ApiClients;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Webapp.Shared.Facades;

namespace Ubik.Accounting.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    public class TestController(IClientContactFacade client) : ControllerBase
    {
        readonly IClientContactFacade client = client;

        [HttpGet("/Hello")]
        public async Task<ActionResult> Hello()
        {
            await client.GetAllAccountsAsync();
            return Ok("Hi!");
        }

        [HttpGet("/Accountslist")]
        public async Task<IEnumerable<GetAllAccountsResult>> AccountList()
        {
            return await client.GetAllAccountsAsync();
        }

    }

 
}
