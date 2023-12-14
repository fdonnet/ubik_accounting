using Ubik.Accounting.Webapp.Shared.Render;

namespace Ubik.Accounting.WebApp.Render
{
    public class ServerRenderContext(IHttpContextAccessor contextAccessor) : IRenderContext
    {
        public bool IsClient => false;
        public bool IsServer => true;
        public bool IsPrerendering => !contextAccessor.HttpContext?.Response.HasStarted ?? false;
    }
}
