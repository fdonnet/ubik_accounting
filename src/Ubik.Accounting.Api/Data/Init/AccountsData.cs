using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Enums;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class AccountsData
    {
        internal static async Task LoadAsync(AccountingContext context)
        {
            if (!context.Accounts.Any())
            {
                //                var baseValuesGeneral = new BaseValuesGeneral();
                //                var baseValuesForTenants = new BaseValuesForTenants();
                //                var baseValuesForUsers = new BaseValuesForUsers();
                //                var baseValuesForAccounts = new BaseValuesForAccounts();
                //                var baseValuesForCurrencies = new BaseValuesForCurrencies();

                //                var accounts = new Account[]
                //                {
                //                    new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-91f9-08dbfb1b9879"),
                //    Code = "1000",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Cash",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-b866-08dbfb1baa3b"),
                //    Code = "1020",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Bank 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-885e-08dbfb1bb84d"),
                //    Code = "1060",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Shares held short",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-70f2-08dbfb1bd313"),
                //    Code = "1100",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Trade receivables",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-0af4-08dbfb1be194"),
                //    Code = "1110",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Other current receivables",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-5a1f-08dbfb1bf0ab"),
                //    Code = "1200",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Inventories, unbilled services",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-c605-08dbfb1bfa0e"),
                //    Code = "1300",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Prepaid expenses",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-c0fd-08dbfb1c0f1e"),
                //    Code = "1400",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Financial asset 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-bc27-08dbfb1c19af"),
                //    Code = "1480",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Participations",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-e3c2-08dbfb1c2a97"),
                //    Code = "1500",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Mobile tangible asset 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-e3c3-08dbfb1c3581"),
                //    Code = "1600",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Immobile tangible asset",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-6ca4-08dbfb1c4463"),
                //    Code = "1700",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Intangible asset 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-911f-08dbfb1c4e5d"),
                //    Code = "1800",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Not paid capital",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Asset,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-999a-08dbfb1c5c38"),
                //    Code = "2000",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Trade payables 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-e5a8-08dbfb1c7977"),
                //    Code = "2200",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Other short-term liability 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-a83a-08dbfb1c6dcb"),
                //    Code = "2100",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Interest current liability 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-84cf-08dbfb1c8fc9"),
                //    Code = "2300",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Deferred income liability",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-d041-08dbfb1cb833"),
                //    Code = "2400",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Long-term liability 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-49ae-08dbfb1cc2eb"),
                //    Code = "2600",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Accrual 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-f62c-08dbfb1cd03a"),
                //    Code = "2800",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Share capital",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-90cc-08dbfb1cda6e"),
                //    Code = "2801",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Shareholder's equity",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-1298-08dbfb1ce36f"),
                //    Code = "2850",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Private benefits",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-5c78-08dbfb1cf663"),
                //    Code = "2950",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Legal reserves",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-7275-08dbfb1d03a3"),
                //    Code = "2900",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Capital reserves",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-7a18-08dbfb1d0d80"),
                //    Code = "2960",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Retained earnings/losses",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Liability,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-25ef-08dbfb1d22f2"),
                //    Code = "3000",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Revenue 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Income,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-8001-08dbfb1d3158"),
                //    Code = "3001",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Revenue 2",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Income,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-a4e9-08dbfb1d3f67"),
                //    Code = "4000",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Cost of sales 1",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-7163-08dbfb1d4eee"),
                //    Code = "4001",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Cost of sales 2",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-01f1-08dbfb1d5aa2"),
                //    Code = "5000",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Salaries and fees",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-ca83-08dbfb1d6950"),
                //    Code = "5700",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Social security expenses",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-20b8-08dbfb1d7527"),
                //    Code = "5800",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Other personnel expenses",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-7424-08dbfb1d8146"),
                //    Code = "6000",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Rental expenses",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-5feb-08dbfb1d965f"),
                //    Code = "6200",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Vehicle expenditure",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-24f3-08dbfb1da0e4"),
                //    Code = "6300",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Assurances",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-e5b7-08dbfb1dab5a"),
                //    Code = "6400",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Expenditure of energy and disposal",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-72bd-08dbfb1db314"),
                //    Code = "6500",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Administration expenditures",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-f78d-08dbfb1dbb86"),
                //    Code = "6600",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Advertising expenditure",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-926a-08dbfb1dc783"),
                //    Code = "6700",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Additional operational expenses",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-994e-08dbfb1dd108"),
                //    Code = "6800",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Depreciations/impairments tangible assets",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-1175-08dbfb1ddd24"),
                //    Code = "6900",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Interest / finance expense",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-32e2-08dbfb1deaed"),
                //    Code = "6950",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Interest / finance income",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Income,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-a8e6-08dbfb1df52a"),
                //    Code = "7000",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " External revenues",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Income,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-a32a-08dbfb1dfdb4"),
                //    Code = "8000",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = " Extraordinary revenues",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Income,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //},
                //new Account
                //{
                //    Id= Guid.Parse("10070000-5d1a-0015-a001-08dbfb1e06dc"),
                //    Code = "8900",
                //    CurrencyId = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d"),
                //    CreatedBy= baseValuesForUsers.UserId1,
                //    CreatedAt = baseValuesGeneral.GenerationTime,
                //    Label = "Taxes",
                //    Description = "",
                //    Category = AccountCategory.General,
                //    Domain = AccountDomain.Charge,
                //    ModifiedBy= baseValuesForUsers.UserId1,
                //    ModifiedAt = baseValuesGeneral.GenerationTime,
                //    TenantId= baseValuesForTenants.TenantId,
                //    Version = NewId.NextGuid()
                //}                };

                var accountsQuery = await File.ReadAllTextAsync(@"Data\Init\AccountsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(accountsQuery));

                //foreach (Account a in accounts)
                //{
                //    context.Accounts.Add(a);
                //}
                //context.SaveChanges();
            }
        }
    }
}
