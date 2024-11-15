namespace Ubik.Accounting.Transaction.Contracts.Txs.Events
{
    public record TxTaxValidationRequestSent
    {
        public required Guid Id { get; init; }
        public required TxSubmitted Tx { get; init; }
    }
}
