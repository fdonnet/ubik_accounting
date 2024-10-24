
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Data.Init;

namespace Ubik.Security.Api.Features.Application.Services
{
    public class ApplicationCommandService(SecurityDbContext ctx, IWebHostEnvironment env) : IApplicationCommandService
    {
        public async Task<bool> CleanupDatabaseInDevAsync()
        {
            try
            {
                if (env.IsDevelopment())
                {
                    await ctx.Database.EnsureDeletedAsync();
                    await ctx.Database.EnsureCreatedAsync();
                    await DbInitializer.InitializeAsync(ctx);
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
