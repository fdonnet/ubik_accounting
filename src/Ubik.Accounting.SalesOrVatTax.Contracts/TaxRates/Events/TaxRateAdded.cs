﻿namespace Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events
{
    public record TaxRateAdded
    {
        public Guid Id { get; init; }
        public DateOnly ValidFrom { get; init; }
        public DateOnly? ValidTo { get; init; }
        public required string Code { get; init; }
        public string? Description { get; init; }
        public Decimal Rate { get; init; }
        public Guid Version { get; init; }
        public Guid TenantId { get; init; }
    }
}
