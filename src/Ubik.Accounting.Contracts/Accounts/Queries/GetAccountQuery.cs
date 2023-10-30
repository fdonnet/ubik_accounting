using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Accounts.Queries
{
    public record GetAccountQuery
    {
        [Required]
        public Guid Id { get; init; }
    }
}
