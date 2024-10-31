namespace Ubik.Accounting.Structure.Api.Data.Init
{
    static internal class DbInitializer
    {
        static internal async Task InitializeAsync(AccountingDbContext context)
        {
            CurrenciesData.Load(context);
            ClassificationsData.Load(context);
            await AccountGroupsData.LoadAsync(context);
            await AccountsData.LoadAsync(context);
            await AccountsAccountGroupsData.LoadAsync(context);
        }
    }
}
