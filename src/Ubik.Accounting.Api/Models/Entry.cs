using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    public enum DebitCredit
    {
        Debit,
        Credit
    }

    public class Entry : ITenantEntity, IConcurrencyCheckEntity
    {
        public Guid Id { get; set; }
        public required DebitCredit Sign { get; set;}
        [Precision(18, 2)]
        public required decimal Amount { get; set; }
        [Precision(18, 2)]
        public decimal? OriginalAmount { get; set; }
        public Guid? OriginalCurrencyId { get; set; }
        public Currency? OriginalCurrency { get; set; }
        [Precision(18, 8)]
        public decimal ExchangeRate { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }
}