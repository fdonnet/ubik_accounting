using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Accounts.Commands
{
    public record AddAccountInAccountGroupCommand
    {
        [Required]
        public Guid AccountId { get; init; }
        [Required]
        public Guid AccountGroupId { get; init; }
    }
}
