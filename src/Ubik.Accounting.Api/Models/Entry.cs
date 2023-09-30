using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ubik.Accounting.Api.Models
{
    [Table("Entries")]
    public class Entry : ITenant
    {
        public int Id { get; set; }
        public required int DebitAccountId { get; set; }
        public Account DebitAccount { get; set; } = default!;
        public required int CreditAccountId { get; set; }
        public Account CreditAccount { get; set; } = default!;
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }
        public int TenantId { get; set; }
    }
}