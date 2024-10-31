using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Structure.Contracts.Classifications.Commands
{
    public record UpdateClassificationCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        [MaxLength(20)]
        public string Code { get; init; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; init; } = default!;
        [MaxLength(700)]
        public string? Description { get; init; }
        [Required]
        public Guid Version { get; init; }
    }
}
