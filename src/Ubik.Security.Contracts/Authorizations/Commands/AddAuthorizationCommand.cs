using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Authorizations.Commands
{
    public record AddAuthorizationCommand
    {
        [Required]
        [MaxLength(50)]
        public required string Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; init; }
        [Required]
        [MaxLength(250)]
        public string? Description { get; init; }
        [Required]
        public bool IsOnlyForMegaAdmin { get; init; } = false;
    }
}
