using LanguageExt;

namespace Ubik.Security.Api.Features.Application.Services
{
    public interface IApplicationCommandService
    {
        public Task<bool> CleanupDatabaseInDevAsync();
    }
}
