using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Events
{
    public class TxRejected
    {
        public required Guid Id { get; init; }
        public required Guid Version { get; init; }
        public required string Reason { get; init; }
    }
}
