using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Commands
{
    public record DeleteClassificationCommand
    {
        [Required]
        public Guid Id { get; init; }
    }
}
