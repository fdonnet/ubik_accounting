namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public class AccountInAccountGroupStandardResult
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
        public Guid Version { get; set; }
    }
}
