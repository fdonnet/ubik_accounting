namespace Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco
{
    public record AccountGroupClassification
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label {  get; init; } = default!;
        public Guid ClassificationId { get; init; }
        public string ClassificationCode { get; init; } = default!;
        public string ClassificationLabel { get; init; } = default!;
    }
}
