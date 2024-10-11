namespace Ubik.Security.Contracts.Tenants.Events
{
    public record TenantDeleted
    {
        public Guid Id { get; init; }
    }
}
