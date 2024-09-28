namespace Ubik.Security.Contracts.Authorizations.Events
{
    public record AuthorizationAdded
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public string? Description { get; init; }
        public required bool IsOnlyForMegaAdmin { get; init; } = false;
        public required Guid Version { get; init; }
    }
}
