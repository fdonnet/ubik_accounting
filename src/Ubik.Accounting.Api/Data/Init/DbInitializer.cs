namespace Ubik.Accounting.Api.Data.Init
{
    internal class DbInitializer
    {
        internal async Task InitializeAsync(AccountingDbContext context)
        {
            CurrenciesData.Load(context);
            ClassificationsData.Load(context);
            await AccountGroupsData.LoadAsync(context);
            await AccountsData.LoadAsync(context);
            await AccountsAccountGroupsData.LoadAsync(context);
        }
    }
}
