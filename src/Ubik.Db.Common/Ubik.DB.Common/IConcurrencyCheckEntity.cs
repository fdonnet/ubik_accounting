using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.DB.Common
{
    public interface IConcurrencyCheckEntity
    {
        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
