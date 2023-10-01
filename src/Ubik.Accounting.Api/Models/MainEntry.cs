using System.ComponentModel.DataAnnotations.Schema;

namespace Ubik.Accounting.Api.Models
{
    [Table("MainEntries")]
    public class MainEntry : Entry
    {
        public required Guid CurrencyId { get; set; }
        public Currency Currency { get; set; } = default!;
        public required DateTime ValueDate { get; set; }
        public ICollection<CounterpartyEntry> CounterpartyEntries { get; set; } = default!;
    }
}
