using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.AccountGroups.Queries
{
    public record GetChildAccountsQuery
    {
        [Required]
        public Guid AccountGroupId { get; init; }
    }
}
