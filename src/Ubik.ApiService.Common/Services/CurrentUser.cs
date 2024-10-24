namespace Ubik.ApiService.Common.Services
{
    public class CurrentUser : ICurrentUser
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public bool IsMegaAdmin { get; set; } = false;
        //TODO:Change that as soon as possible, it's only to not crash all my previous accounting api
        public Guid? TenantId { get; set; } = new Guid("74a20000-088f-d0ad-7a4e-08dce86b0459");
    }
}
