﻿using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public interface IGetAllAccountGroupsResult
    {
        GetAllAccountGroupsResult[] AccountGroups { get; }
    }
}

