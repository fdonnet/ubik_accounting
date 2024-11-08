
using Ubik.Accounting.Transaction.Api.Data;
using Ubik.Accounting.Transaction.Api.Data.Init;

namespace Ubik.Accounting.Transaction.Api.Features.Application.Services
{
    public class ApplicationCommandService(AccountingTxContext ctx, IWebHostEnvironment env) : IApplicationCommandService
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
