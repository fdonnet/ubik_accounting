using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.AccountGroups.Queries
{
    public record GetAccountGroupQuery
    {
        [Required]
        public Guid Id { get; init; }
    }
}
