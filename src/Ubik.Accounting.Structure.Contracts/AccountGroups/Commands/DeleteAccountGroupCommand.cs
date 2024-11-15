using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Structure.Contracts.AccountGroups.Commands
{
    public record DeleteAccountGroupCommand
    {
        [Required]
        public Guid Id { get; init; }
    }
}
