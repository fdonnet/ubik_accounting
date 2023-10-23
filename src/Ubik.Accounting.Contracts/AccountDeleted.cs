using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts
{
    public record AccountDeleted
    {
        public Guid Id { get; set; }
    }
}
