using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Xml.Linq;

namespace Ubik.ApiService.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private ICurrentUser _currentUser = null;

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
                var identity = _contextAccessor.HttpContext?.User.Identity as ClaimsIdentity;

                if (identity != null)
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
            if (_currentUser == null)
                _currentUser = new CurrentUser() { Email = "fake@fake.com", Name = "fake", TenantIds = new Guid[] { Guid.NewGuid() }, Id = Guid.NewGuid() };
            
            return _currentUser;
        }
    }
}
