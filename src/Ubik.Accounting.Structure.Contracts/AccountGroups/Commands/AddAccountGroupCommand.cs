using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Structure.Contracts.AccountGroups.Commands
{
    public record AddAccountGroupCommand
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; init; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; init; } = default!;
        [MaxLength(700)]
        public string? Description { get; init; }
        public Guid? ParentAccountGroupId { get; init; }
        [Required]
        public Guid AccountGroupClassificationId { get; init; }
    }
}
