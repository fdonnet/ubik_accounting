using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ubik.ApiService.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        //TODO: need to be replaced by a call to User Grpc + caching
        //TODO: adapt to query the DB and use caching
        public ICurrentUser GetCurrentUser()
        {
            return new CurrentUser() { 
                Email = "test@gmail.com", 
                Id = Guid.Parse("7f16c899-536e-4792-87c0-0af35ebdadac"), 
                Name = "testuser", 
                TenantId= Guid.Parse("c1546e5c-3e30-46be-a24c-e6852c907868") };
        }
    }
}
