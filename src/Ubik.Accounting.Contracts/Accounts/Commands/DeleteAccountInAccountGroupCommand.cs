using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Accounts.Commands
{
    public record DeleteAccountInAccountGroupCommand
    {
        [Required]
        public Guid AccountId { get; init; }
        [Required]
        public Guid AccountGroupId { get; init; }
    }
}
