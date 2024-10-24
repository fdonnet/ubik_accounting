namespace Ubik.Security.Contracts.UserTenants.Events
{
    public record UserTenantAdded
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public Guid Version { get; init; }
    }
}