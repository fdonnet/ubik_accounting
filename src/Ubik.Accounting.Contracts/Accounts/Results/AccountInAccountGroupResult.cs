namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public class AccountInAccountGroupResult
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
        public Guid Version { get; set; }
    }
}
