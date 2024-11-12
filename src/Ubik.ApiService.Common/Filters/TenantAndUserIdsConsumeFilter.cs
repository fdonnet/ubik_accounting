using MassTransit;
using Ubik.ApiService.Common.Services;

//TODO: change to be adapted to select tenantId not first tenantid of the list
namespace Ubik.ApiService.Common.Filters
{
    public class TenantAndUserIdsConsumeFilter<T>(ICurrentUser currentUser) :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var tenantId = context.Headers.Get<string>("TenantId");
            var userId = context.Headers.Get<string>("UserId");

            currentUser.TenantId = Guid.TryParse(tenantId, out var setTenantId) 
                ? setTenantId 
                : default;

            currentUser.Id = Guid.TryParse(userId, out var setUserId)
                ? setUserId
                : default;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
