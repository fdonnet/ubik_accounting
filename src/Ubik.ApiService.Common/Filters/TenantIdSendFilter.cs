using MassTransit;
using Ubik.ApiService.Common.Services;

//TODO: change to be adapted to select tenantId not first tenantid of the list
namespace Ubik.ApiService.Common.Filters
{
    public class TenantIdSendFilter<T>(ICurrentUser currentUse) :
        IFilter<SendContext<T>>
        where T : class
    {
        public void Probe(ProbeContext context)
        {

        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            context.Headers.Set("TenantId", currentUse.TenantId.ToString());

            return next.Send(context);
        }
    }
}
