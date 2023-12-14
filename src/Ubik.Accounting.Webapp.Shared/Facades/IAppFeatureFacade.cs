namespace Ubik.Accounting.Webapp.Shared.Facades
{
    /// <summary>
    /// The Facade layer is an abstraction layer for communication
    /// between the Components and Application depending on the Blazor RenderMode.
    /// 
    /// Server Facades use WebSockets for requests; Client Facades use HTTP for requests.
    /// </summary>
    public interface IAppFeatureFacade { }
}
