namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record AddClassificationResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public string? Description { get; init; }
        public Guid Version { get; init; }
    }
}
