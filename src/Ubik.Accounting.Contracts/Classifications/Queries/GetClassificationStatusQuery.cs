using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Queries
{
    public record GetClassificationStatusQuery
    {
        public Guid Id { get; init; }
    }
}
