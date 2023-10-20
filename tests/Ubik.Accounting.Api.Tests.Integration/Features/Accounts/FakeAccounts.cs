using Bogus;
using Bogus.Extensions;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public static class FakeGenerator
    {
        public static IEnumerable<Account> GenerateAccounts(int numTests)
        {
            return new Faker<Account>("fr_CH")
                 .CustomInstantiator(a => new Account()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                 }).Generate(numTests);
        }

        public static IEnumerable<AccountGroup> GenerateAccountGroups(int numTests)
        {
            var testData = new BaseValuesForAccountGroupClassifications();
            return new Faker<AccountGroup>("fr_CH")
                 .CustomInstantiator(a => new AccountGroup()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                     AccountGroupClassificationId = testData.AccountGroupClassificationId1
                 }).Generate(numTests);
        }
    }
}
