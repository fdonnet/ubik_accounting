using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Structure.Api.Features.Classifications.Services;
using Ubik.Accounting.Structure.Api.Mappers;
using Ubik.Accounting.Structure.Contracts.Accounts.Results;
using Ubik.Accounting.Structure.Contracts.Classifications.Commands;
using Ubik.Accounting.Structure.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Structure.Api.Features.Classifications.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ClassificationsController(IClassificationQueryService queryService, IClassificationCommandService commandService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<ClassificationStandardResult>>> GetAll()
        {
            var results = (await queryService.GetAllAsync()).ToClassificationStandardResults();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<ClassificationStandardResult>> Get(Guid id)
        {
            var result = await queryService.GetAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToClassificationStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        /// <summary>
        /// Get all accounts attached to an account group in the classification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/accounts")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<AccountStandardResult>>> GetAttachedAccounts(Guid id)
        {
            var result = await queryService.GetClassificationAttachedAccountsAsync(id);

            return result.Match(
                            Right: ok => Ok(ok.ToAccountStandardResults()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        /// <summary>
        /// Get all accounts not attached to an account group in the classification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/missingaccounts")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<AccountStandardResult>>> GetMissingAccounts(Guid id)
        {
            var result = await queryService.GetClassificationMissingAccountsAsync(id);

            return result.Match(
                            Right: ok => Ok(ok.ToAccountStandardResults()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        /// <summary>
        /// Get the status of a classification (ready to be used or incomplete = missing accounts)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<ClassificationStatusResult>> GetStatus(Guid id)
        {
            var result = await queryService.GetClassificationStatusAsync(id);

            return result.Match(
                            Right: ok => Ok(ok.ToClassificationStatusResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<ClassificationStandardResult>> Add(AddClassificationCommand command)
        {
            var result = await commandService.AddAsync(command);

            return result.Match(
                            Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok.ToClassificationStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<ClassificationStandardResult>> Update(Guid id, UpdateClassificationCommand command)
        {
            if (command.Id != id)
                return new ObjectResult(new ResourceIdNotMatchForUpdateError("Classification",id, command.Id)
                    .ToValidationProblemDetails(HttpContext));

            var result = await commandService.UpdateAsync(command);

            return result.Match(
                            Right: ok => Ok(ok.ToClassificationStandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        /// <summary>
        /// Delete classification and all the linked account groups
        /// </summary>
        /// <remarks>Return All the account groups removed</remarks>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<ClassificationDeleteResult>> Delete(Guid id)
        {
            var result = await commandService.DeleteAsync(id);

            return result.Match(
                            Right: ok => Ok(ok.ToClassificationDeleteResult(id)),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

    }
}
