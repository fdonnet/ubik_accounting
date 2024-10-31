using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Structure.Contracts.Classifications.Commands
{
    public record DeleteClassificationCommand
    {
        [Required]
        public Guid Id { get; init; }
    }
}
