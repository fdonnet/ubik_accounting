using Ubik.Accounting.Transaction.Contracts.Entries.Enums;
using Ubik.Accounting.Transaction.Contracts.Txs.Enums;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Events
{
    public record class TxSubmitted
    {
        public required Guid Id { get; init; }
        public required TxStateInfoSubmitted State { get; init; }
        public required DateOnly ValueDate { get; init; }
        public required decimal Amount { get; init; }
        public IEnumerable<TxEntrySubmitted> Entries { get; init; } = default!;
        public required Guid Version { get; init; }
    }

    public record TxEntrySubmitted
    {
        public required Guid Id { get; init; }
        public required EntryType Type { get; init; }
        public required DebitCredit Sign { get; init; }
        public required Guid AccountId { get; init; }
        public string? Label { get; init; }
        public string? Description { get; init; }
        public required decimal Amount { get; init; }
        public TxEntryAdditionalAmountInfoSubmitted? AmountAdditionnalInfo { get; init; }
        public TxEntryTaxInfoSubmitted? TaxInfo { get; init; } = default!;
        public required Guid Version { get; init; }
    }

    public record TxEntryAdditionalAmountInfoSubmitted
    {
        public required decimal OriginalAmount { get; init; }
        public required Guid OriginalCurrencyId { get; init; }
        public decimal ExchangeRate { get; init; }
    }

    public record TxEntryTaxInfoSubmitted
    {
        public required decimal TaxAppliedRate { get; init; }
        public required Guid TaxRateId { get; init; }
    }

    public record TxStateInfoSubmitted
    {
      public required TxState State { get; init; }
      public string? Reason { get; init; }
    }
}
