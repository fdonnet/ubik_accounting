namespace Ubik.Accounting.Api.Data.Init
{
    public class DbInitializer
    {
        public async Task InitializeAsync(AccountingContext context)
        {
            CurrenciesData.Load(context);
            ClassificationsData.Load(context);
            await AccountGroupsData.LoadAsync(context);
            await AccountsData.LoadAsync(context);
            await AccountsAccountGroupsData.LoadAsync(context);
        }
    }
}
