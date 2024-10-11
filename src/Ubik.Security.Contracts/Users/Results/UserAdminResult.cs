using Ubik.Security.Contracts.Authorizations.Results;

namespace Ubik.Security.Contracts.Users.Results
{
    public record UserAdminResult
    {
        public Guid Id { get; init; }
        public required string Firstname { get; init; }
        public required string Lastname { get; init; }
        public required string Email { get; init; }
        public bool IsActivated { get; init; } = true;
        public bool IsMegaAdmin { get; init; } = false;
        public Guid Version { get; init; }
        public required Dictionary<Guid,List<AuthorizationStandardResult>> AuthorizationsByTenantIds { get; init; }
    }
}
