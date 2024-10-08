using MassTransit;
using Ubik.ApiService.Common.Services;

//TODO: change to be adapted to select tenantId not first tenantid of the list
namespace Ubik.ApiService.Common.Filters
{
    public class TenantIdConsumeFilter<T>(ICurrentUser currentUser) :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var tenantId = context.Headers.Get<string>("TenantId");

            currentUser.TenantId = Guid.TryParse(tenantId, out var setTenantID) 
                ? setTenantID 
                : default;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
