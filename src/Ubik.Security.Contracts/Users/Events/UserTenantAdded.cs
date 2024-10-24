using Ubik.Security.Contracts.Tenants.Events;

namespace Ubik.Security.Contracts.Users.Events
{
    public record UserTenantAdded
    {
        public Guid UserId { get; init; }
        public required TenantAdded NewLinkedTenantCreated { get; init; }
    }
}
