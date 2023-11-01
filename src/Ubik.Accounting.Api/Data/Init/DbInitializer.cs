namespace Ubik.Accounting.Api.Data.Init
{
    public class DbInitializer
    {
        public void Initialize(AccountingContext context)
        {
            CurrenciesData.Load(context);
            ClassificationsData.Load(context);
            AccountGroupsData.Load(context);
            AccountsData.Load(context);
            AccountsAccountGroupsData.Load(context);
        }
    }
}
