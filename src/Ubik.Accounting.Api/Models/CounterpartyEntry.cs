using System.ComponentModel.DataAnnotations.Schema;

namespace Ubik.Accounting.Api.Models
{
    [Table("CounterpartyEntries")]
    public class CounterpartyEntry : Entry
    {
        public Guid MainEntryId { get; set; }
        public MainEntry MainEntry { get; set; } = default!;
    }
}
