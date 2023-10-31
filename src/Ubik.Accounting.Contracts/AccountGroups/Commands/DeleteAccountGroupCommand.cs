using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Contracts.AccountGroups.Commands
{
    public record DeleteAccountGroupCommand : IRequest<bool>
    {
        [Required]
        public Guid Id { get; init; }
    }
}
