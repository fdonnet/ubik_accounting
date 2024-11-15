namespace Ubik.Accounting.Structure.Contracts.Accounts.Results
{
    public record AccountGroupWithClassificationResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public Guid ClassificationId { get; init; }
        public string ClassificationCode { get; init; } = default!;
        public string CLassificationLabel { get; init; } = default!;
    }
}
