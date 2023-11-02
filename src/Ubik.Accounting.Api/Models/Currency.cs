using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    //TODO: will be updated by another service
    public class Currency : ITenantEntity, IConcurrencyCheckEntity
    {
        public Guid Id { get; set; }
        [StringLength(3)]
        public required string IsoCode { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }
}
