using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Services;

//TODO: change to be adapted to select tenantId not first tenantid of the list
namespace Ubik.ApiService.Common.Filters
{
    public class TenantIdSendFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        private ICurrentUserService _userService;

        public TenantIdSendFilter(ICurrentUserService userService)
        {
            _userService = userService;
        }

        public void Probe(ProbeContext context)
        {

        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            context.Headers.Set("TenantId", _userService.CurrentUser.TenantIds[0].ToString());

            return next.Send(context);
        }
    }
}
