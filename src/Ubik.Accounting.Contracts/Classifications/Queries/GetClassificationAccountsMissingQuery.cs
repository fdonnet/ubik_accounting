using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Classifications.Queries
{
    public record GetClassificationAccountsMissingQuery
    {
        [Required]
        public Guid ClassificationId { get; init; }
    }
}
