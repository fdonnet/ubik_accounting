using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Services
{
    public interface ICurrentUser
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        Guid TenantId { get; set; }

    }
}
