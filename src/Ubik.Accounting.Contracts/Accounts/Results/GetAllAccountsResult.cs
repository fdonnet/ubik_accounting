using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record GetAllAccountsResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Label { get; set; } = default!;
        public AccountCategory Category { get; set; }
        public AccountDomain Domain { get; set; }
        public string? Description { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid Version { get; set; }
    }
}
