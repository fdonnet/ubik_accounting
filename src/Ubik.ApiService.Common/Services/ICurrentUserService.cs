using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Services
{
    public interface ICurrentUserService
    {        
        //TODO : transform that to async call
        ICurrentUser GetCurrentUser();
    }
}
