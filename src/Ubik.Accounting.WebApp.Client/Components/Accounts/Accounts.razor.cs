using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.WebApp.Client.Components.Accounts
{
    public partial class Accounts
    {
        //Used to re-import base data in DB
        private void PrintToConsole(IEnumerable<AccountModel> accounts)
        {
            var modelPrint = string.Empty;
            
            foreach (var account in accounts)
            {
                modelPrint +=
                    $$"""
                    new Account
                    {
                        Id= Guid.Parse("{{account.Id}}"),
                        Code = "{{account.Code}}",
                        CurrencyId = Guid.Parse("{{account.CurrencyId}}"),
                        CreatedBy= baseValuesForUsers.UserId1,
                        CreatedAt = baseValuesGeneral.GenerationTime,
                        Label = "{{account.Label}}",
                        Description = "{{account.Description}}",
                        Category = AccountCategory.{{account.Category}},
                        Domain = AccountDomain.{{account.Domain}},
                        ModifiedBy= baseValuesForUsers.UserId1,
                        ModifiedAt = baseValuesGeneral.GenerationTime,
                        TenantId= baseValuesForTenants.TenantId,
                        Version = NewId.NextGuid()
                    },
                    """;

                modelPrint += Environment.NewLine;
            }
            Console.WriteLine(modelPrint);
        }
    }
}
