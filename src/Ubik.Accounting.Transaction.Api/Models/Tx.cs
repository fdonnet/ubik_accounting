﻿using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.Transaction.Api.Models
{
    public class Tx : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required DateOnly ValueDate { get; set; }
        public required string Label { get; set; }
        public decimal Amount { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
