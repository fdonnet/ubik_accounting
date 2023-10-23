namespace Ubik.ApiService.Common.Services
{
    public class CurrentUser : ICurrentUser
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required Guid[] TenantIds { get; set; }
    }
}
