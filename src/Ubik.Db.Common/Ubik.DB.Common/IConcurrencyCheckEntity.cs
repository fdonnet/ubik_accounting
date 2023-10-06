using System.ComponentModel.DataAnnotations;

namespace Ubik.DB.Common
{
    public interface IConcurrencyCheckEntity
    {
        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
