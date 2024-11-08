using Ubik.Accounting.Transaction.Contracts.Entries.Enums;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Events
{
    public record class TxSubmited
    {
        public required Guid Id { get; init; }
        public required DateOnly ValueDate { get; init; }
        public required decimal Amount { get; init; }
        public IEnumerable<TxEntrySubmited> Entries { get; init; } = default!;
    }

    public record TxEntrySubmited
    {
        public required Guid Id { get; init; }
        public required EntryType Type { get; init; }
        public required DebitCredit Sign { get; init; }
        public required Guid AccountId { get; init; }
        public string? Label { get; init; }
        public string? Description { get; init; }
        public required decimal Amount { get; init; }
        public TxEntryAdditionalAmountInfoSubmited? AmountAdditionnalInfo { get; init; }
        public TxEntryTaxInfoSubmited? TaxInfo { get; init; } = default!;
    }

    public record TxEntryAdditionalAmountInfoSubmited
    {
        public required decimal OriginalAmount { get; init; }
        public required Guid OriginalCurrencyId { get; init; }
        public decimal ExchangeRate { get; init; }
    }

    public record TxEntryTaxInfoSubmited
    {
        public required decimal TaxAppliedRate { get; init; }
        public required Guid TaxRateId { get; init; }
    }
}
