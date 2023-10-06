using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Exceptions
{
    public class ProblemDetailErrors
    {
        public required string Code { get; set; }
        public required string FriendlyMsg { get; set; }
        public required string ValueInError { get; set; }
    }
}
