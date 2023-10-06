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
                Id = Guid.Parse("2b470b86-ead8-4f52-b609-c67595665c50"), 
                Name = "testuser", 
                TenantId= Guid.Parse("7a16e50a-04fc-4af9-be08-d8295519400c") };
        }
    }
}
