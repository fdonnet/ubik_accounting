namespace Ubik.Accounting.Structure.Api.Features.Application.Services
{
    public interface IApplicationCommandService
    {
        public Task<bool> CleanupDatabaseInDevAsync();
    }
}
