using Ubik.Accounting.Structure.Contracts.Accounts.Results;

namespace Ubik.Accounting.Structure.Contracts.Classifications.Results
{
    public class ClassificationStatusResult
    {
        public Guid Id { get; init; }
        public bool IsReady { get; init; }
        public IEnumerable<AccountStandardResult> MissingAccounts { get; init; } = default!;
    }
}
