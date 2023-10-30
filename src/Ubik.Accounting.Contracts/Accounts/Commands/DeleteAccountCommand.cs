using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.Accounts.Commands
{
    public record DeleteAccountCommand
    {
        [Required]
        public Guid Id { get; init; }
    }
}
