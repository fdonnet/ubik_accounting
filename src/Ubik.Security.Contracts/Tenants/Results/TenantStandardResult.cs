namespace Ubik.Security.Contracts.Tenants.Results
{
    public record TenantStandardResult
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public required string Description { get; init; }
        public bool IsActivated { get; init; }
        public Guid Version { get; init; }
    }
}
