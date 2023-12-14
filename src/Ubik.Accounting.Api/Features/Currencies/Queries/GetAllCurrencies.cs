using MassTransit;
using Ubik.Accounting.Api.Features.Currencies.Mappers;
using Ubik.Accounting.Contracts.Currencies.Queries;
using Ubik.Accounting.Contracts.Currencies.Results;

namespace Ubik.Accounting.Api.Features.Currencies.Queries
{
    public class GetAllCurrenciesConsumer : IConsumer<GetAllCurrenciesQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetAllCurrenciesConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public async Task Consume(ConsumeContext<GetAllCurrenciesQuery> context)
        {
            var res = await _serviceManager.CurrencyService.GetAllAsync();
            await context.RespondAsync(new GetAllCurrenciesResults
            {
                Currencies = res.ToGetAllCurrenciesResult()
            });
        }
    }
}
