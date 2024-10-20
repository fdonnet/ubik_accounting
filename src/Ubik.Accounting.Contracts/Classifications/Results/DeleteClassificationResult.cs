using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record DeleteClassificationResult
    {
        public Guid Id { get; init; }
        public IEnumerable<AccountGroupStandardResult> DeletedAccountGroups { get; init; } = [];
    }
}
