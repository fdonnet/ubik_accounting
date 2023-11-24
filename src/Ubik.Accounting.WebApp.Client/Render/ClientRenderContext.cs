using Ubik.Accounting.Webapp.Shared.Render;

namespace Ubik.Accounting.WebApp.Client.Render
{
    public sealed class ClientRenderContext : IRenderContext
    {
        public bool IsClient => true;
        public bool IsServer => false;
        public bool IsPrerendering => false;
    }
}
