using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Controller.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ClassificationsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ClassificationsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = "ubik_accounting_classification_read")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(CustomProblemDetails), 500)]
        public async Task<ActionResult<IEnumerable<GetAllClassificationsResult>>> GetAll()
        {
            var results = (await _serviceManager.ClassificationService.GetAllAsync()).ToGetAllClassificationsResult();
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
            var result = await _serviceManager.ClassificationService.GetAsync(id);
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
        public async Task<ActionResult<GetClassificationAccountsResult>> GetAccounts(Guid id)
        {
            var result = await _serviceManager.ClassificationService.GetClassificationAccountsAsync(id);
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
            var result = await _serviceManager.ClassificationService.GetClassificationAccountsMissingAsync(id);
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
            var result = await _serviceManager.ClassificationService.GetClassificationStatusAsync(id);
            return result.Match(
                            Right: ok => Ok(ok.ToGetClassificationStatusResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
        }

    }
}
