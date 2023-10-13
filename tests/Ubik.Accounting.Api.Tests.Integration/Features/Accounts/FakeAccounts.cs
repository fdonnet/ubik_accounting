using Bogus;
using Bogus.Extensions;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Tests.Integration.Features.Accounts
{
    public static class FakeGenerator
    {
        public static IEnumerable<Account> GenerateAccounts(int numTests, Guid accountGroupId)
        {
            return new Faker<Account>("fr_CH")
                 .CustomInstantiator(a => new Account()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                     AccountGroupId = accountGroupId
                 }).Generate(numTests);
        }
    }
}
