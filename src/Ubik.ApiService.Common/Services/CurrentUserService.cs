using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Xml.Linq;

//TODO: don't forget to change the fake value or recode that for the moment we put the correct tenant_id to allow initial data check on model creating !!!!
namespace Ubik.ApiService.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private ICurrentUser _currentUser = default!;

        public ICurrentUser CurrentUser
        {
            get { return GetCurrentUser(); }
        }


        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        //TODO: need to be replaced by a call to User Grpc + caching
        //TODO: adapt to query the DB and use caching
        private ICurrentUser GetCurrentUser()
        {
            if (_currentUser == null)
            {
                if (_contextAccessor.HttpContext?.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;

                    var tenantIds = claims.Where(i => i.Type == "tenantids")?.Select(v => v.Value);
                    var id = claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;

                    if (tenantIds != null && id != null)
                    {
                        _currentUser = new CurrentUser()
                        {
                            Id = Guid.Parse(id),
                            Name = claims.FirstOrDefault(i => i.Type == ClaimTypes.Name)?.Value ?? "",
                            Email = claims.FirstOrDefault(i => i.Type == ClaimTypes.Email)?.Value ?? "",
                            TenantIds = tenantIds == null ? new Guid[] { Guid.NewGuid() } : tenantIds.Select(t => Guid.Parse(t)).ToArray()
                        };
                    }
                }
            }
            //TODO: remove and adapt => cannot keep this fake return
            _currentUser ??= new CurrentUser() 
            { 
                Email = "fake@fake.com", 
                Name = "fake", 
                TenantIds = new Guid[] { Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d") }, 
                Id = Guid.NewGuid() 
            };
            
            return _currentUser;
        }
    }
}
