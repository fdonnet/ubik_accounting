using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Services;

//TODO: change filter thing to be set on selected tenant id (not the first)
namespace Ubik.ApiService.Common.Filters
{
    public class TenantIdPublishFilter<T> :
        IFilter<PublishContext<T>>
        where T : class
    {
        private ICurrentUserService _userService;

        public TenantIdPublishFilter(ICurrentUserService userService)
        {
            _userService = userService;
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            context.Headers.Set("TenantId", _userService.CurrentUser.TenantIds[0].ToString());

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
