using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ubik.Accounting.Api.Models
{
    public class Currency
    {
        public Guid Id { get; set; }
        [StringLength(3)]
        public required string IsoCode { get; set; }
    }
}
