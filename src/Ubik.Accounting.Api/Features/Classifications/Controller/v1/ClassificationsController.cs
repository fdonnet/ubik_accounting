using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Controller.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ClassificationsController(IServiceManager serviceManager) : ControllerBase
    {
        [Authorize(Roles = "ubik_accounting_classification_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllClassificationsResult>>> GetAll()
        {
            var results = (await serviceManager.ClassificationService.GetAllAsync()).ToGetAllClassificationsResult();
            return Ok(results);
        }

        [Authorize(Roles = "ubik_accounting_classification_read")]
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetClassificationResult>> Get(Guid id)
        {
            var result = await serviceManager.ClassificationService.GetAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToGetClassificationResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        /// <summary>
        /// Get all accounts attached to an account group in the classification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ubik_accounting_classification_read")]
        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("{id}/Accounts")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetClassificationAccountsResult>>> GetAccounts(Guid id)
        {
            var result = await serviceManager.ClassificationService.GetClassificationAccountsAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToGetClassificationAccountsResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        /// <summary>
        /// Get all accounts not attached to an account group in the classification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ubik_accounting_classification_read")]
        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("{id}/MissingAccounts")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetClassificationAccountsMissingResult>> GetMissingAccounts(Guid id)
        {
            var result = await serviceManager.ClassificationService.GetClassificationAccountsMissingAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToGetClassificationAccountsMissingResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        /// <summary>
        /// Get the status of a classification (ready to be used or incomplete = missing accounts)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ubik_accounting_classification_read")]
        [Authorize(Roles = "ubik_accounting_account_read")]
        [HttpGet("{id}/Status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<GetClassificationStatusResult>> GetStatus(Guid id)
        {
            var result = await serviceManager.ClassificationService.GetClassificationStatusAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToGetClassificationStatusResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

        [Authorize(Roles = "ubik_accounting_classification_write")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<AddClassificationResult>> Add(AddClassificationCommand command, IRequestClient<AddClassificationCommand> client)
        {
            var (result, error) = await client.GetResponse<AddClassificationResult, IServiceAndFeatureError>(command);

            if (result.IsCompletedSuccessfully)
            {
                var ok = (await result).Message;
                return CreatedAtAction(nameof(Get), new { id = ok.Id }, ok);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

        [Authorize(Roles = "ubik_accounting_classification_write")]
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 409)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<UpdateClassificationResult>> Update(Guid id,
            UpdateClassificationCommand command, IRequestClient<UpdateClassificationCommand> client)
        {
            if (command.Id != id)
                return new ObjectResult(new ResourceIdNotMatchForUpdateError("Classification",id, command.Id)
                    .ToValidationProblemDetails(HttpContext));


            var (result, error) = await client.GetResponse<UpdateClassificationResult, IServiceAndFeatureError>(command);

            if (result.IsCompletedSuccessfully)
            {
                var updated = (await result).Message;
                return Ok(updated);
            }
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

        /// <summary>
        /// Delete classification and all the linked account groups
        /// </summary>
        /// <remarks>Return All the account groups removed</remarks>
        /// <param name="id"></param>
        /// <param name="client"></param>
        [Authorize(Roles = "ubik_accounting_classification_write")]
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        [ProducesResponseType(typeof(CustomProblemDetails), 404)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<DeleteClassificationResult>> Delete(Guid id, IRequestClient<DeleteClassificationCommand> client)
        {
            var (result, error) = await client.GetResponse<DeleteClassificationResult,
            IServiceAndFeatureError>(new DeleteClassificationCommand { Id = id });

            if (result.IsCompletedSuccessfully)
                return Ok((await result).Message);
            else
            {
                var problem = await error;
                return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
            }
        }

    }
}
