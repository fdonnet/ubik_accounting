namespace Ubik.Accounting.Webapp.Shared.Render
{
    public interface IRenderContext
    {
        /// <summary>
        /// Rendering from the Client project. Using HTTP request for connectivity.
        /// </summary>
        public bool IsClient { get; }

        /// <summary>
        /// Rendering from the Server project. Using WebSockets for connectivity.
        /// </summary>
        public bool IsServer { get; }

        /// <summary>
        /// Rendering from the Server project. Indicates if the response has started rendering.
        /// </summary>
        public bool IsPrerendering { get; }
    }
}
