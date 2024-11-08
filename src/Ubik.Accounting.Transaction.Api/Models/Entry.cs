using Ubik.Accounting.Transaction.Contracts.Entries.Enums;
using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.Transaction.Api.Models
{
    public class Entry : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required EntryType Type { get; set; }
        public required DebitCredit Sign { get; set; }
        public required Guid TxId { get; set; }
        public required Guid AccountId { get; set; }
        //Used to keep a trace of the VAT rate applied to the entry (at a time)
        public string? Label { get; set; }
        public string? Description { get; set; }
        //See if we want the amount with or without VAT
        public required decimal Amount { get; set; }
        public AmountAdditionalInfo? AmountAdditionnalInfo { get; set; }
        public TaxInfo? TaxInfo { get; set; } = default!;
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
