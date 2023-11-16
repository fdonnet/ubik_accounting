using Microsoft.Extensions.DependencyInjection;
using Ubik.Accounting.Api.Data;

namespace Ubik.Accounting.Api.Tests.Integration
{
    [Collection("AuthServer and DB")]
    public abstract class BaseIntegrationTest
        : IDisposable
    {
        private readonly IServiceScope _scope;
        public readonly AccountingContext DbContext;
        public IntegrationTestWebAppFactory Factory { get; }
       

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            Factory = factory;
            _scope = factory.Services.CreateScope();

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
