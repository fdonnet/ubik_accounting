﻿using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Transaction.Api.Features.Txs.Services;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TxsController(ITxCommandService commandService) : ControllerBase
    {
        [HttpPost("submit")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<TxSubmitted>> SubmitTx(SubmitTxCommand command)
        {
            var result = await commandService.SubmitTxAsync(command);
            return result.Match(
                            Right: ok => Ok(ok),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }
    }
}
