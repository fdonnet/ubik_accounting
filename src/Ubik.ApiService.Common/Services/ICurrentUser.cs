namespace Ubik.ApiService.Common.Services
{
    public interface ICurrentUser
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        Guid[] TenantIds { get; set; }

    }
}
