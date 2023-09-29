using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ubik.Accounting.Api.Models
{
    [Table("Entries")]
    public class Entry
    {
        public int Id { get; set; }
        public required int DebitAccountId { get; set; }
        public required Account DebitAccount { get; set; }
        public required int CreditAccountId { get; set; }
        public required Account CreditAccount { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
    }
}