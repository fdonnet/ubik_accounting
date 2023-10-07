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
                Id = Guid.Parse("9124f11f-20dd-4888-88f8-428e59bbc53e"), 
                Name = "testuser", 
                TenantId= Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d") };
        }
    }
}
