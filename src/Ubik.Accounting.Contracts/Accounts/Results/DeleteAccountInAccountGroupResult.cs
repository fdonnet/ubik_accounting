namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record DeleteAccountInAccountGroupResult
    {
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
    }
}
