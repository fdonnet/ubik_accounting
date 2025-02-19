﻿using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.SalesOrVatTax.Api.Models
{
    public class TaxRate : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidTo { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public decimal Rate { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
