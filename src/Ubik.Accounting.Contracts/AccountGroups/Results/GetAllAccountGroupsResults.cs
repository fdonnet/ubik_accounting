﻿namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public record GetAllAccountGroupsResults
    {
        public GetAllAccountGroupsResult[] AccountGroups { get; init; } = default!;
    }
}

