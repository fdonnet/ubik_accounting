using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Classifications.Queries
{
    public record GetClassificationQuery
    {
        [Required]
        public Guid Id { get; init; }
    }
}
