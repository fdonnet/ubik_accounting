using MassTransit;
using Ubik.Security.Api.Features.Users.Mappers;
using Ubik.Security.Contracts.Users.Queries;

namespace Ubik.Security.Api.Features.Users.Queries
{
    public class GetUserConsumer(IServiceManager serviceManager) : IConsumer<GetUserQuery>
    {
        public async Task Consume(ConsumeContext<GetUserQuery> context)
        {
            var result = await serviceManager.UserManagementService.GetAsync(context.Message.Id);

            await result.Match(
                Right: async ok => await context.RespondAsync(ok.ToGetUserResult()),
                Left: async err => await context.RespondAsync(err));
        }
    }
}
