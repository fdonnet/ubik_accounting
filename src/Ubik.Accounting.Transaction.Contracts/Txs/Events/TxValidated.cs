using Ubik.Accounting.Transaction.Contracts.Entries.Enums;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Events
{
    public record class TxValidated
    {
        public required Guid Id { get; init; }
        public required Guid Version { get; init; }
    }
}
