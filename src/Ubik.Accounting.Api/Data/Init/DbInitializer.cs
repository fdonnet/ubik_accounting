using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    public class DbInitializer
    {
        public void Initialize(AccountingContext context)
        {
            UsersData.Load(context);
            AccountGroupClassificationsData.Load(context);
            AccountGroupsData.Load(context);
            AccountsData.Load(context);
        }
    }
}
