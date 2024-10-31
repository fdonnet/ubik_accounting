using Ubik.Accounting.Contracts.Accounts.Enums;

namespace Ubik.Accounting.Contracts.Accounts.Events
{
    public record AccountUpdated
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public AccountCategory Category { get; init; }
        public AccountDomain Domain { get; init; }
        public bool Active { get; init; } = true;
        public string? Description { get; init; }
        public Guid CurrencyId { get; init; }
        public Guid Version { get; init; }
        public Guid TenantId { get; init; }
    }
}
