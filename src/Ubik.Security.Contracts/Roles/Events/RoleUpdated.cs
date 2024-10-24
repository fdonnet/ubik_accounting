namespace Ubik.Security.Contracts.Roles.Events
{
    public record RoleUpdated
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public string? Description { get; init; }
        public Guid Version { get; init; }
    }
}
