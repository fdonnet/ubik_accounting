namespace Ubik.Accounting.Contracts.Error
{
    public record ProblemDetailsContract
    {
        public string Type { get; init; } = default!;
        public string Title { get; init; } = default!;
        public int Status { get; init; }
        public string Detail { get; init; } = default!;
        public string Instance { get; init; } = default!;
        public ProblemDetailErrorContract[] Errors { get; init; } = [];
    }

    public record ProblemDetailErrorContract
    {
        public string Code { get; init; } = default!;
        public string FriendlyMsg { get; init; } = default!;
        public string ValueInError { get; init; } = default!;
    }
}
