using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Classifications.Queries
{
    public record GetClassificationStatusQuery
    {
        [Required]
        public Guid Id { get; init; }
    }
}
