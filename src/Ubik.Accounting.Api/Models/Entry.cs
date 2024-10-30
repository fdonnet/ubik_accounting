using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    public enum DebitCredit
    {
        Debit,
        Credit
    }

    public enum EntryType
    {
        Main,
        Counterparty,
    }

    public class Entry : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required EntryType Type { get; set; }
        public required DebitCredit Sign { get; set; }
        public required Guid TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        public required Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public required decimal Amount { get; set; }
        public decimal? OriginalAmount { get; set; }
        public Guid? OriginalCurrencyId { get; set; }
        public Currency? OriginalCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
