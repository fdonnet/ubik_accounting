using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Transaction.Contracts.Entries.Enums;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Events
{
    public record class TxSubmited
    {
        public Guid Id { get; init; }
        public required DateOnly ValueDate { get; init; }
        public required decimal Amount { get; init; }
        public IEnumerable<TxEntrySubmited> Entries { get; init; } = default!;
    }

    public record TxEntrySubmited
    {
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
