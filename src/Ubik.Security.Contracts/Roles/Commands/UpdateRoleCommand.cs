using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Roles.Commands
{
    public record UpdateRoleCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        [MaxLength(20)]
        public required string Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; init; }
        [MaxLength(700)]
        public string? Description { get; init; }
        [Required]
        public Guid Version { get; init; }
    }
}
