using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Queries
{
    public record GetClassificationAccountsQuery
    {
        [Required]
        public Guid ClassificationId { get; init; }
    }
}
