using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco
{
    public record ClassificationStatus
    {
        public Guid Id { get; init; }
        public bool IsReady { get; init; }

        public IEnumerable<Account> MissingAccounts { get; init; } = default!;
    }
}
