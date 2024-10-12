using Microsoft.Extensions.DependencyInjection;
using Ubik.Accounting.Api.Data;

namespace Ubik.Api.Tests.Integration
{
    [Collection("AuthServer and DB")]
    public abstract class BaseIntegrationTestOld
        : IDisposable
    {
        private readonly IServiceScope _scope;
        public readonly AccountingDbContext DbContext;
        public IntegrationTestAccoutingFactory Factory { get; }



        internal BaseIntegrationTestOld(IntegrationTestAccoutingFactory factory)
        {
            Factory = factory;
            _scope = factory.Services.CreateScope();

            DbContext = _scope.ServiceProvider
                .GetRequiredService<AccountingDbContext>();
           
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
