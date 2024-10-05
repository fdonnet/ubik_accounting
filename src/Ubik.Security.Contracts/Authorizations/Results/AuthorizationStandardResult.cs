namespace Ubik.Security.Contracts.Authorizations.Results
{
    public record AuthorizationStandardResult
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public string? Description { get; init; }
        public required Guid Version { get; init; }
    }
}
