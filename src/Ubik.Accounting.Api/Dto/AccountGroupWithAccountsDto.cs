using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Dto
{
    public class AccountGroupWithAccountsDto
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public string? Description { get; set; }
        public ICollection<Account>? Accounts { get; set; }
    }
}
