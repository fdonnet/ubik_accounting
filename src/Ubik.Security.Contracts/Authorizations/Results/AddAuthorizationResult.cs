namespace Ubik.Security.Contracts.Authorizations.Results
{
    public record AddAuthorizationResult
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public string? Description { get; init; }
        public required bool IsOnlyForMegaAdmin { get; init; } = false;
        public required Guid Version { get; init; }
    }
}
