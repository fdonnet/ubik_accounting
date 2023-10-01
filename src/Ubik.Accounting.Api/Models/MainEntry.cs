using System.ComponentModel.DataAnnotations.Schema;

namespace Ubik.Accounting.Api.Models
{
    [Table("MainEntries")]
    public class MainEntry : Entry
    {
        public ICollection<CounterpartyEntry> CounterpartyEntries { get; set; } = default!;
    }
}
