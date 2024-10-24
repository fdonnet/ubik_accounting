namespace Ubik.Accounting.Api.Features.Application.Services
{
    public interface IApplicationCommandService
    {
        public Task<bool> CleanupDatabaseInDevAsync();
    }
}
