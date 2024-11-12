using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Transaction.Contracts.Entries.Enums;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Events
{
    public record class TxAdded
    {
        public required Guid Id { get; init; }
        public required DateOnly ValueDate { get; init; }
        public required decimal Amount { get; init; }
        public IEnumerable<TxEntryAdded> Entries { get; init; } = default!;
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }

    public record TxEntryAdded
    {
        public required Guid Id { get; init; }
        public required EntryType Type { get; init; }
        public required DebitCredit Sign { get; init; }
        public required Guid AccountId { get; init; }
        public string? Label { get; init; }
        public string? Description { get; init; }
        public required decimal Amount { get; init; }
        public TxEntryAdditionalAmountInfoAdded? AmountAdditionnalInfo { get; init; }
        public TxEntryTaxInfoAdded? TaxInfo { get; init; } = default!;
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }

    public record TxEntryAdditionalAmountInfoAdded
    {
        public required decimal OriginalAmount { get; init; }
        public required Guid OriginalCurrencyId { get; init; }
        public decimal ExchangeRate { get; init; }
    }

    public record TxEntryTaxInfoAdded
    {
        public required decimal TaxAppliedRate { get; init; }
        public required Guid TaxRateId { get; init; }
    }
}
