using Microsoft.EntityFrameworkCore;

namespace Ubik.Accounting.Structure.Api.Data.Init
{
    static internal class DbInitializer
    {
        static internal async Task InitializeAsync(AccountingDbContext context)
        {
            await CurrenciesData.LoadAsync(context);
            ClassificationsData.Load(context);
            await AccountGroupsData.LoadAsync(context);
            await AccountsData.LoadAsync(context);
            await AccountsAccountGroupsData.LoadAsync(context);

            //TODO: need to evolve to other health check for PROD
            //Set the application as ready
            await context.Database.ExecuteSqlRawAsync("UPDATE application SET is_ready = true");
        }
    }
}
