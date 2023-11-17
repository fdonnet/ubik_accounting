namespace Ubik.Accounting.Contracts.Classifications.Events
{
    public record ClassificationAdded
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public string? Description { get; init; }
        public Guid Version { get; init; }
    }
}
