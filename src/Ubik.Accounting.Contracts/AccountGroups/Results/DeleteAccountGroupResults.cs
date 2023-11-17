namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public record DeleteAccountGroupResults
    {
        public IEnumerable<DeleteAccountGroupResult> AccountGroups { get; init; } = default!;
    }
}
