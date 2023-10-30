using MassTransit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Services;

//TODO: change to be adapted to select tenantId not first tenantid of the list
namespace Ubik.ApiService.Common.Filters
{
    public class TenantIdConsumeFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        private ICurrentUserService _userService;

        public TenantIdConsumeFilter(ICurrentUserService userService)
        {
            _userService = userService;
        }

        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var tenantId = context.Headers.Get<string>("TenandId");

            if (Guid.TryParse(tenantId, out var setTenantID))
                _userService.CurrentUser.TenantIds[0] = setTenantID;
            else
                _userService.CurrentUser.TenantIds[0] = default(Guid);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
