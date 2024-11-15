using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Features.Classifications.CustomPoco
{
    public record ClassificationStatus
    {
        public Guid Id { get; init; }
        public bool IsReady { get; init; }

        public IEnumerable<Account> MissingAccounts { get; init; } = default!;
    }
}
