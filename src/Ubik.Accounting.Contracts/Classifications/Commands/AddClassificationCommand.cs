using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Commands
{
    public record AddClassificationCommand
    {
        [Required]
        public string Code { get; init; } = default!;
        [Required]
        public string Label { get; init; } = default!;
        public string? Description { get; init; }
    }
}
