using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    [Index(nameof(TenantId), IsUnique = false)]
    [Table("Currencies")]
    public class Currency : ITenantEntity, IConcurrencyCheckEntity
    {
        public Guid Id { get; set; }
        [StringLength(3)]
        public required string IsoCode { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }
}
