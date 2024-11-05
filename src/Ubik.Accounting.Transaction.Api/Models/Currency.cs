using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Transaction.Api.Models
{
    //Source of truth => To be determined
    public class Currency
    {
        public Guid Id { get; set; }
        [StringLength(3)]
        public required string IsoCode { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }
}
