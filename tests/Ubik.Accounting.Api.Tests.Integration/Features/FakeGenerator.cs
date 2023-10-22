﻿using Bogus;
using Bogus.Extensions;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Models;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.AddAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.UpdateAccountGroup;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;

namespace Ubik.Accounting.Api.Tests.Integration.Features
{
    public static class FakeGenerator
    {
        public static IEnumerable<AddAccountCommand> GenerateAddAccounts(int numTests)
        {
            return new Faker<AddAccountCommand>("fr_CH")
                 .CustomInstantiator(a => new AddAccountCommand()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                 }).Generate(numTests);
        }
        public static IEnumerable<UpdateAccountCommand> GenerateUpdAccounts(int numTests)
        {
            return new Faker<UpdateAccountCommand>("fr_CH")
                 .CustomInstantiator(a => new UpdateAccountCommand()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                 }).Generate(numTests);
        }

        public static IEnumerable<AddAccountGroupCommand> GenerateAddAccountGroups(int numTests)
        {
            var testData = new BaseValuesForAccountGroupClassifications();
            return new Faker<AddAccountGroupCommand>("fr_CH")
                 .CustomInstantiator(a => new AddAccountGroupCommand()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                     AccountGroupClassificationId = testData.AccountGroupClassificationId1
                 }).Generate(numTests);
        }

        public static IEnumerable<UpdateAccountGroupCommand> GenerateUpdAccountGroups(int numTests)
        {
            var testData = new BaseValuesForAccountGroupClassifications();
            return new Faker<UpdateAccountGroupCommand>("fr_CH")
                 .CustomInstantiator(a => new UpdateAccountGroupCommand()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                     AccountGroupClassificationId = testData.AccountGroupClassificationId1
                 }).Generate(numTests);
        }

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