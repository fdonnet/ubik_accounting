using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ubik.Accounting.Api.Models
{
    [Table("AccountGroups")]
    public class AccountGroup
    {
        public int Id { get; set; }
        [StringLength(100)]
        public required string Label { get; set; }
        [StringLength(700)]
        public string? Description { get; set; }
        public int? ParentAccountGroupId { get; set; }
        public AccountGroup? ParentAccountGroup { get; set; }
        public ICollection<AccountGroup>? ChildrenAccountGroups { get; set; }
        public ICollection<Account>? Accounts { get; set; }
    }
}
