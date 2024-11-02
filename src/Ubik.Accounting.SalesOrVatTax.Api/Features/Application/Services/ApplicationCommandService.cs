using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Data.Init;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Application.Services
{
    public class ApplicationCommandService(AccountingSalesTaxDbContext ctx, IWebHostEnvironment env) : IApplicationCommandService
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
