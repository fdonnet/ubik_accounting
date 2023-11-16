using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Accounts.Queries
{
    public record GetAccountGroupsForAccountQuery
    {
        [Required]
        public Guid AccountId { get; init; }
    }
}
