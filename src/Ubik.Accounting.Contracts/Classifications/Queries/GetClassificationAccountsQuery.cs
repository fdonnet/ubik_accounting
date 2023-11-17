using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Classifications.Queries
{
    public record GetClassificationAccountsQuery
    {
        [Required]
        public Guid ClassificationId { get; init; }
    }
}
