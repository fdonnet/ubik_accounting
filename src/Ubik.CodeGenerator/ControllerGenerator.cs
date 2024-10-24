using Ubik.Security.Api.Data;

namespace Ubik.CodeGenerator
{
    internal class ControllerGenerator(SecurityDbContext dbContext)
    {
        public void GenerateController(string? type = null)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                                  && e.ClrType.Name != "OutboxMessage"
                                                                                  && e.ClrType.Name != "OutboxState");
            if (type != null)
                entityTypes = entityTypes.Where(e => e.ClrType.Name == type);

            foreach (var entityType in entityTypes)
            {
                var className = entityType.ClrType.Name;
                var classContent = GetTemplateForController().Replace("{ClassName}", className);

                Console.WriteLine(classContent);
            }
        }
        private static string GetTemplateForController()
        {
            return
                """
                [ApiController]
                [ApiVersion("1.0")]
                [Route("api/v{version:apiVersion}/[controller]")]
                public class {ClassName}sController(I{ClassName}sCommandsService commandService, I{ClassName}sQueriesService queryService) : ControllerBase
                {
                    [HttpGet]
                    [ProducesResponseType(200)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 400)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 500)]
                    public async Task<ActionResult<IEnumerable<{ClassName}StandardResult>>> GetAll()
                    {
                        var results = (await queryService.GetAllAsync()).To{ClassName}StandardResults();
                        return Ok(results);
                    }

                    [HttpGet("{id}")]
                    [ProducesResponseType(200)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 400)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 404)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 500)]
                    public async Task<ActionResult<{ClassName}StandardResult>> Get(Guid id)
                    {
                        var result = await queryService.GetAsync(id);
                        return result.Match(
                                        Right: ok => Ok(ok.To{ClassName}StandardResult()),
                                        Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
                    }

                    [HttpPost]
                    [ProducesResponseType(201)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 400)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 409)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 500)]
                    public async Task<ActionResult<{ClassName}StandardResult>> AddAsync(Add{ClassName}Command command)
                    {
                        var result = await commandService.AddAsync(command);

                        return result.Match(
                            Right: ok => CreatedAtAction(nameof(Get), new { id = ok.Id }, ok.To{ClassName}StandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
                    }

                    [HttpPut("{id}")]
                    [ProducesResponseType(200)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 400)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 404)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 409)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 500)]
                    public async Task<ActionResult<{ClassName}StandardResult>> Update(Guid id, Update{ClassName}Command command)
                    {
                        if (command.Id != id)
                            return new ObjectResult(new ResourceIdNotMatchForUpdateError("{ClassName}", id, command.Id)
                                .ToValidationProblemDetails(HttpContext));

                        var result = await commandService.UpdateAsync(command);

                        return result.Match(
                            Right: ok => Ok(ok.To{ClassName}StandardResult()),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
                    }

                    [HttpDelete("{id}")]
                    [ProducesResponseType(204)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 400)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 404)]
                    [ProducesResponseType(typeof(CustomProblemDetails), 500)]
                    public async Task<ActionResult> Delete(Guid id)
                    {
                        var result = await commandService.ExecuteDeleteAsync(id);

                        return result.Match<ActionResult>(
                            Right: ok => NoContent(),
                            Left: err => new ObjectResult(err.ToValidationProblemDetails(HttpContext)));
                    }
                }
                """;
        }
    }
}
