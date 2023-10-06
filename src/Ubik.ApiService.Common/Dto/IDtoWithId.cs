using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Dto
{
    public interface IDtoWithId
    {
        public Guid Id { get; set; }
    }
}
