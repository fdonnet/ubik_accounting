using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Structure.Contracts.Accounts.Commands
{
    public record DeleteAccountInAccountGroupCommand
    {
        [Required]
        public Guid AccountId { get; init; }
        [Required]
        public Guid AccountGroupId { get; init; }
    }
}
