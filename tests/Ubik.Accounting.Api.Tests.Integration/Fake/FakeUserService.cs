using MassTransit;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Tests.Integration.Fake
{
    public class FakeUserService : ICurrentUser
    {
        public Guid Id { get; set; } = NewId.NextGuid();
        public Guid? TenantId { get; set; } = Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d");
        public bool IsMegaAdmin { get; set; } = false;
    }
}
