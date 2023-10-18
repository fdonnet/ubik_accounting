using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ubik.Accounting.Api.Data;

namespace Ubik.Accounting.Api.Tests.Integration
{
    [Collection("Keycloack and DB")]
    public abstract class BaseIntegrationTest
        : IDisposable
    {
        private readonly IServiceScope _scope;
        protected readonly ISender Sender;
        public readonly AccountingContext DbContext;
        public IntegrationTestWebAppFactory Factory { get; }

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            Factory = factory;
            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

            DbContext = _scope.ServiceProvider
                .GetRequiredService<AccountingContext>();
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            _scope?.Dispose();
            DbContext?.Dispose();
        }
    }
}
