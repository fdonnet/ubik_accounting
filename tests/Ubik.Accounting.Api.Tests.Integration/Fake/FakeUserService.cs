using MassTransit;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Tests.Integration.Fake
{
    public class FakeUserService : ICurrentUserService
    {
        private ICurrentUser _currentUser = default!;

        public ICurrentUser CurrentUser
        {
            get { return GetCurrentUser(); }
        }


        //TODO: need to be replaced by a call to User Grpc + caching
        //TODO: adapt to query the DB and use caching --- maybe, we will see (the auth claims can be sufficent)
        private ICurrentUser GetCurrentUser()
        {
            //TODO: remove and adapt => cannot keep this fake return
            _currentUser ??= new CurrentUser()
            {
                Email = "fake@fake.com",
                Name = "fake",
                TenantIds = new Guid[] { Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d") },
                Id = NewId.NextGuid()
            };

            return _currentUser;
        }
    }
}
