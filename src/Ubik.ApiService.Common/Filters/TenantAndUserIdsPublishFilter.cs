﻿using MassTransit;
using Ubik.ApiService.Common.Services;

//TODO: change filter thing to be set on selected tenant id (not the first)
namespace Ubik.ApiService.Common.Filters
{
    public class TenantAndUserIdsPublishFilter<T>(ICurrentUser currentUser) :
        IFilter<PublishContext<T>>
        where T : class
    {
        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            context.Headers.Set("TenantId", currentUser.TenantId.ToString());
            context.Headers.Set("UserId", currentUser.Id.ToString());

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}