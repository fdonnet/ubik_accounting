namespace Ubik.Accounting.Structure.Contracts.Classifications.Events
{
    public class ClassificationUpdated
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public string? Description { get; init; }
        public Guid Version { get; init; }
        public Guid TenantId { get; init; }
    }
}
