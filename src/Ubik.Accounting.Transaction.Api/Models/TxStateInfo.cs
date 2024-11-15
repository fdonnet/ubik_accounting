using Ubik.Accounting.Transaction.Contracts.Txs.Enums;

namespace Ubik.Accounting.Transaction.Api.Models
{
    public class TxStateInfo
    {
        public TxState State { get; set; }
        public string? Reason { get; set; }
    }
}
