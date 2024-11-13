using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Transaction.Contracts.Txs.Enums;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Commands
{
    public record ChangeTxStateCommand
    {
        public Guid TxId { get; init; }
        public Guid Version { get; init; }
        public TxState State { get; init; }
        public string? Reason { get; init; }
    }
}
