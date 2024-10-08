namespace Ubik.ApiService.Common.Services
{
    public interface ICurrentUser
    {
        Guid Id { get; set; }
        bool IsMegaAdmin { get; set; }
        Guid? TenantId { get; set; }
    }
}
