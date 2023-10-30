using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record AddAccountResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public AccountCategory Category { get; init; }
        public AccountDomain Domain { get; init; }
        public string? Description { get; init; }
        public Guid CurrencyId { get; init; }
        public Guid Version { get; init; }
    }
}
