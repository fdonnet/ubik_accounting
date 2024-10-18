﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Exceptions;
using Asp.Versioning;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using MassTransit;
using Ubik.ApiService.Common.Errors;
using Ubik.Accounting.Api.Features.Mappers;
using Ubik.Accounting.Api.Features.AccountGroups.Services;

namespace Ubik.Accounting.Api.Features.AccountGroups.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountGroupsController(IServiceManager serviceManager, IAccountGroupQueryService queryService) : ControllerBase
    {
        //TODO: add auhtorization (maybe manage that in API security before)
        //[Authorize(Roles = "ubik_accounting_accountgroup_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<AccountGroupStandardResult>>> GetAll()
        {
            var result = (await queryService.GetAllAsync()).ToAccountGroupStandardResults();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountGroupResult>> Get(Guid id)
        {
            var result = await queryService.GetAsync(id);

            return result.Match(
                Right: ok => Ok(ok.ToAccountGroupStandardResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpGet("{id}/Accounts")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetAccountGroupResult>> GetChildAccount(Guid id)
        {
            var result = await serviceManager.AccountGroupService.GetChildAccountsAsync(id);

            return result.Match(
                Right: r => Ok(r.ToGetChildAccountsResult()),
                Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddAccountGroupResult>> Add(AddAccountGroupCommand command, IRequestClient<AddAccountGroupCommand> client)
        {
            var (result, error) = await client.GetResponse<AddAccountGroupResult, IServiceAndFeatureError>(command);

            if (result.IsCompletedSuccessfully)
            {
                var addedAccountGroup = (await result).Message;
                return CreatedAtAction(nameof(Get), new { id = addedAccountGroup.Id }, addedAccountGroup);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UpdateAccountGroupResult>> Update(Guid id,
            UpdateAccountGroupCommand command, IRequestClient<UpdateAccountGroupCommand> client)
        {
            if (command.Id != id)
                return new ObjectResult(new ResourceIdNotMatchForUpdateError("AccountGroup",id, command.Id)
                    .ToValidationProblemDetails(HttpContext));
            

            var (result, error) = await client.GetResponse<UpdateAccountGroupResult, IServiceAndFeatureError>(command);

            if (result.IsCompletedSuccessfully)
            {
                var updatedAccountGroup = (await result).Message;
                return Ok(updatedAccountGroup);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

        /// <summary>
        /// Delete account groups with all children
        /// </summary>
        /// <remarks>Return All the account groups removed</remarks>
        /// <param name="id"></param>
        /// <param name="client"></param>
        [Authorize(Roles = "ubik_accounting_accountgroup_write")]
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<DeleteAccountGroupResult>>> Delete(Guid id, IRequestClient<DeleteAccountGroupCommand> client)
        {
            var (result, error) = await client.GetResponse<DeleteAccountGroupResults,
            IServiceAndFeatureError>(new DeleteAccountGroupCommand { Id = id });

            if (result.IsCompletedSuccessfully)
                return Ok((await result).Message.AccountGroups);
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

    }
}
