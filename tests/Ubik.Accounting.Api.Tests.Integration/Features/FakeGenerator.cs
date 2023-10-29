﻿using Bogus;
using Bogus.Extensions;
using MassTransit.DependencyInjection.Registration;
using Ubik.Accounting.Api.Data.Init;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.ApiService.DB.Enums;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.AddAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.UpdateAccountGroup;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;

namespace Ubik.Accounting.Api.Tests.Integration.Features
{
    public static class FakeGenerator
    {
        public static IEnumerable<AddAccountCommand> GenerateAddAccounts(int numTests)
        {
            var testData = new BaseValuesForCurrencies();
            return new Faker<AddAccountCommand>("fr_CH")
                 .CustomInstantiator(a => new AddAccountCommand()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                     CurrencyId = testData.CurrencyId1,
                     Category = AccountCategory.General,
                     Domain = AccountDomain.Asset
                 }).Generate(numTests);
        }
        public static IEnumerable<UpdateAccountCommand> GenerateUpdAccounts(int numTests, Guid id = default,
            string? code = null, string? label = null, Guid version = default)

        {
            var testData = new BaseValuesForCurrencies();
            return new Faker<UpdateAccountCommand>("fr_CH")
                 .CustomInstantiator(a => new UpdateAccountCommand()
                 {
                     Id = id != default ? id : default,
                     Code = code ?? a.Finance.Account().ToString(),
                     Label = label ?? a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                     CurrencyId = testData.CurrencyId1,
                     Category = AccountCategory.General,
                     Domain = AccountDomain.Asset,
                     Version = version != default ? version : default
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

        public static IEnumerable<Account> GenerateAccounts(int numTests, Guid currencyId)
        {
            return new Faker<Account>("fr_CH")
                 .CustomInstantiator(a => new Account()
                 {
                     Code = a.Finance.Account().ToString(),
                     Label = a.Finance.AccountName().ClampLength(1, 100),
                     Description = a.Lorem.Paragraphs().ClampLength(1, 700),
                     Category = AccountCategory.General,
                     Domain = AccountDomain.Asset,
                     CurrencyId = currencyId
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
