namespace Ubik.ApiService.Common.Services
{
    public interface ICurrentUserService
    {        
        //TODO : transform that to async call
        ICurrentUser GetCurrentUser();
    }
}
