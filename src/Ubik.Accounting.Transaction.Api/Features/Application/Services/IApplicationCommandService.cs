namespace Ubik.Accounting.Transaction.Api.Features.Application.Services
{
    public interface IApplicationCommandService
    {
        public Task<bool> CleanupDatabaseInDevAsync();
    }
}
