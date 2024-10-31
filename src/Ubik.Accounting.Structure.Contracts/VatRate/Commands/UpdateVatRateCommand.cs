using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Structure.Contracts.VatRate.Commands
{
    public record UpdateVatRateCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
        [Required]
        [MaxLength(20)]
        public required string Code { get; init; }
        [MaxLength(200)]
        public string? Description { get; init; }
        [Required]
        public Decimal Rate { get; init; }
        [Required]
        public Guid Version { get; init; }
    }
}
