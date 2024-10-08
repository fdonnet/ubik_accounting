using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Tenants.Commands
{
    public record UpdateTenantCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        [MaxLength(50)]
        public required string Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; init; }
        [Required]
        [MaxLength(250)]
        public required string Description { get; init; }
        [Required]
        public Guid Version { get; init; }
    }
}
