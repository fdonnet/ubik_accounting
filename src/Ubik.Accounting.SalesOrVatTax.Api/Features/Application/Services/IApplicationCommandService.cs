namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Application.Services
{
    public interface IApplicationCommandService
    {
        public Task<bool> CleanupDatabaseInDevAsync();
    }
}
