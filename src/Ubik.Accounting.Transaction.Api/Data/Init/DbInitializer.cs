namespace Ubik.Accounting.Transaction.Api.Data.Init
{
    static internal class DbInitializer
    {
        static internal async Task InitializeAsync(AccountingTxContext context)
        {
            await AccountsData.LoadAsync(context);
        }
    }
}
