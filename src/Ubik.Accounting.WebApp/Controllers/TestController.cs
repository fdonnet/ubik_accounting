using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ubik.Accounting.WebApp.ApiClients;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Webapp.Shared.Facades;
using System.Net.Http;

namespace Ubik.Accounting.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    public class TestController(IAccountingApiClient client) : ControllerBase
    {
        readonly IAccountingApiClient client = client;

        [HttpGet("/Hello")]
        public async Task<ActionResult> Hello()
        {
            await client.GetAllAccountsAsync();
            return Ok("Hi!");
        }

        [HttpGet("/GetAllAccounts")]
        public async Task<ActionResult<IEnumerable<GetAllAccountsResult>>> AccountList()
        {
            var response = await client.GetAllAccountsAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetAllAccountsResult>>();
                return Ok(result);
            }
            else
            {
                //TODO: write error handling here with problem details etc (onyl forward it to UI)
                //in case of big error throw an exception or send a problem detail back need to be seen.
                return new ObjectResult("ERROR")
                {
                    StatusCode = (int?)response.StatusCode,
                };
            }
        }

    }

 
}
