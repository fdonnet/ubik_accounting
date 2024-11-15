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

        //Not really a command...
        public async Task<bool> IsReady()
        {
            if (env.IsDevelopment())
            {
                await Task.CompletedTask;
                return ctx.Application.IsReady;
            }

            //TODO: implement health check for Live Env. (maybe to check different queries and commands in the controller)
            return true;
        }
    }
}
