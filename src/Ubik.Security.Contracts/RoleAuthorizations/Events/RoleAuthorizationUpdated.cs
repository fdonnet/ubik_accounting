namespace Ubik.Security.Contracts.RoleAuthorizations.Events
{
    public record RoleAuthorizationUpdated
    {
        public Guid Id { get; init; }
        public Guid RoleId { get; init; }
        public Guid AuthorizationId { get; init; }
        public Guid Version { get; init; }
    }
}
