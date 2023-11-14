using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Queries
{
    public record GetClassificationAccountsMissingQuery
    {
        [Required]
        public Guid ClassificationId { get; init; }
    }
}
