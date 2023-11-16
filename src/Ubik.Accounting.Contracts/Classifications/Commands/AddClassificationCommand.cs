using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Classifications.Commands
{
    public record AddClassificationCommand
    {
        [Required]
        public string Code { get; init; } = default!;
        [Required]
        public string Label { get; init; } = default!;
        public string? Description { get; init; }
    }
}
