namespace Ubik.ApiService.Common.Services
{
    public class CurrentUser : ICurrentUser
    {
        public Guid Id { get; set; }
        public bool IsMegaAdmin { get; set; } = false;
        //TODO:Change that as soon as possible, it's only to not crash all my previous accounting api
        public Guid? TenantId { get; set; } = new Guid("727449e8-e93c-49e6-a5e5-1bf145d3e62d");
    }
}
