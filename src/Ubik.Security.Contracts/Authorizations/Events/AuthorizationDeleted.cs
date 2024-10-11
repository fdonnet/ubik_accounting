namespace Ubik.Security.Contracts.Authorizations.Events
{
    public record AuthorizationDeleted
    {
        public Guid Id { get; init; }
    }
}
