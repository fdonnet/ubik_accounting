﻿namespace Ubik.Security.Contracts.Tenants.Events
{
    public record TenantUpdated
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public required string Description { get; init; }
        public Guid Version { get; init; }
    }
}
