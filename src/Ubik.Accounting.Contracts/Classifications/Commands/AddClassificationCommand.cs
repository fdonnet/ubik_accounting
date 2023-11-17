using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Classifications.Commands
{
    public record AddClassificationCommand
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; init; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; init; } = default!;
        [MaxLength(700)]
        public string? Description { get; init; }
    }
}
