﻿using Ubik.Accounting.Structure.Contracts.Accounts.Enums;

namespace Ubik.Accounting.WebApp.Client.Components.Accounts
{
    public class AccountFiltersModel
    {
        public bool IsFiltersApplied { get; set; } = false;
        public Guid? CurrencyFilter { get; set; } = null;
        public AccountDomain? DomainFilter { get; set; } = null;
        public AccountCategory? CategoryFilter { get; set; } = null;
        public bool ShowInactive { get; set; } = false;

    }
}
