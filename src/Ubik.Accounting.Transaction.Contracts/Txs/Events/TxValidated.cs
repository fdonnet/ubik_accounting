using Ubik.Accounting.Transaction.Contracts.Entries.Enums;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Events
{
    public record class TxValidated
    {
        public required Guid Id { get; init; }
        public required DateOnly ValueDate { get; init; }
        public required decimal Amount { get; init; }
        public IEnumerable<TxEntryValidated> Entries { get; init; } = default!;
    }

    public record TxEntryValidated
    {
        public required Guid Id { get; init; }
        public required EntryType Type { get; init; }
        public required DebitCredit Sign { get; init; }
        public required Guid AccountId { get; init; }
        public string? Label { get; init; }
        public string? Description { get; init; }
        public required decimal Amount { get; init; }
        public TxEntryAdditionalAmountInfoValidated? AmountAdditionnalInfo { get; init; }
        public TxEntryTaxInfoValidated? TaxInfo { get; init; } = default!;
    }

    public record TxEntryAdditionalAmountInfoValidated
    {
        public required decimal OriginalAmount { get; init; }
        public required Guid OriginalCurrencyId { get; init; }
        public decimal ExchangeRate { get; init; }
    }

    public record TxEntryTaxInfoValidated
    {
        public required decimal TaxAppliedRate { get; init; }
        public required Guid TaxRateId { get; init; }
    }
}
