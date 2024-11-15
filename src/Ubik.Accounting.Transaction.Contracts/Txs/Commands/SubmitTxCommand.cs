using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Transaction.Contracts.Entries.Enums;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Commands
{
    public record SubmitTxCommand
    {
        [Required]
        public required DateOnly ValueDate { get; init; }
        [Required]
        public required decimal Amount { get; init; }
        public IEnumerable<SubmitTxEntry> Entries { get; init; } = default!;
    }

    public record SubmitTxEntry
    {
        [Required]
        public required EntryType Type { get; init; }
        [Required]
        public required DebitCredit Sign { get; init; }
        [Required]
        public required Guid AccountId { get; init; }
        [MaxLength(100)]
        public string? Label { get; init; }
        [MaxLength(700)]
        public string? Description { get; init; }
        [Required]
        public required decimal Amount { get; init; }
        public SubmitTxEntryAdditionalAmountInfo? AmountAdditionnalInfo { get; init; }
        public SubmitTxEntryTaxInfo? TaxInfo { get; init; } = default!;
    }

    public record SubmitTxEntryAdditionalAmountInfo
    {
        [Required]
        public required decimal OriginalAmount { get; init; }
        [Required]
        public required Guid OriginalCurrencyId { get; init; }
        [Required]
        public decimal ExchangeRate { get; init; }
    }

    public record SubmitTxEntryTaxInfo
    {
        [Required]
        public required decimal TaxAppliedRate { get; init; }
        [Required]
        public required Guid TaxRateId { get; init; }
    }
}
