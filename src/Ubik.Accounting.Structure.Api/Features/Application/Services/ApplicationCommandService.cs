using Ubik.Accounting.Structure.Api.Data;
using Ubik.Accounting.Structure.Api.Data.Init;

namespace Ubik.Accounting.Structure.Api.Features.Application.Services
{
    public class ApplicationCommandService(AccountingDbContext ctx, IWebHostEnvironment env) : IApplicationCommandService
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

        public async Task<bool> IsReady()
        {
            await Task.CompletedTask;
            return ctx.Application.IsReady;
        }
    }
}
