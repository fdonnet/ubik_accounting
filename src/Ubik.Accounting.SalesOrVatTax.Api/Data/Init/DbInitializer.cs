namespace Ubik.Accounting.SalesOrVatTax.Api.Data.Init
{
    static internal class DbInitializer
    {
        static internal async Task InitializeAsync(AccountingSalesTaxDbContext context)
        {
            await TaxRatesData.LoadAsync(context);
        }
    }
}
