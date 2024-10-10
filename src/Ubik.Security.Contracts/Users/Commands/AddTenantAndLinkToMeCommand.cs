using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Security.Contracts.Tenants.Commands;

namespace Ubik.Security.Contracts.Users.Commands
{
    public record AddTenantAndLinkToMeCommand
    {
        [Required]
        [MaxLength(50)]
        public required string Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; init; }
        [Required]
        [MaxLength(250)]
        public required string Description { get; init; }
    }
}
