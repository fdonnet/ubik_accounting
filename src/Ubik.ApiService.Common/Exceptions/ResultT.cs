using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Exceptions
{
    public record ResultT<T>
    {
        public bool IsSuccess { get; init; }
        public T Result { get; init; } = default!;
        public IServiceAndFeatureException Exception { get; init; } = default!;
    }
}
