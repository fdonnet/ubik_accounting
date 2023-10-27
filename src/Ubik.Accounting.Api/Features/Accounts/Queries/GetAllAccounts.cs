using MassTransit;
using MediatR;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAllAccounts
    {
        public record GetAllAccountsQuery 
        {

        }

        public record GetAllAccountsResult
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = default!;
            public string Label { get; set; } = default!;
            public AccountCategory Category { get; set; }
            public AccountDomain Domain { get; set; }
            public string? Description { get; set; }
            public Guid CurrencyId { get; set; }
            public Guid Version { get; set; }
        }

        public interface IGetAllAccountsResult
        {
            GetAllAccountsResult[] Accounts { get; }
        }

        //public class GetAllAccountsHandler : IRequestHandler<GetAllAccountsQuery, IEnumerable<GetAllAccountsResult>>
        //{
        //    private readonly IServiceManager _serviceManager;

        //    public GetAllAccountsHandler(IServiceManager serviceManager)
        //    {
        //        _serviceManager = serviceManager;
        //    }

        //    public async Task<IEnumerable<GetAllAccountsResult>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        //    {
        //        var accounts = await _serviceManager.AccountService.GetAllAsync();
        //        return accounts.ToGetAllAccountResult();
        //    }
        //}

        public class GetAllAccountsConsumer : IConsumer<GetAllAccountsQuery>
        {
            private readonly IServiceManager _serviceManager;

            public GetAllAccountsConsumer(IServiceManager serviceManager)
            {
                _serviceManager = serviceManager;
            }
            public async Task Consume(ConsumeContext<GetAllAccountsQuery> context)
            {
                var accounts = await _serviceManager.AccountService.GetAllAsync();
                await context.RespondAsync<IGetAllAccountsResult>(new
                {
                    Accounts = accounts.ToGetAllAccountResult()
                });
            }
        }
    }
}
