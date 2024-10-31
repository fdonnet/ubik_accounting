using Ubik.Accounting.Structure.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Structure.Contracts.Classifications.Results
{
    public record ClassificationDeleteResult
    {
        public Guid Id { get; init; }
        public IEnumerable<AccountGroupStandardResult> DeletedAccountGroups { get; init; } = [];
    }
}
