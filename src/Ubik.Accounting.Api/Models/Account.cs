using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Api.Models
{
    [Index(nameof(Code), IsUnique = true)]
    public class Account : ITenant
    {
        public int Id { get; set; }
        [StringLength(20)]
        public required string Code { get; set; }
        [StringLength(100)]
        public required string Label { get; set; }
        [StringLength(700)]
        public  string? Description { get; set; }
        public int? AccountGroupId { get; set; }
        public AccountGroup? Group { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }
        public int TenantId { get; set; }

    }
}
