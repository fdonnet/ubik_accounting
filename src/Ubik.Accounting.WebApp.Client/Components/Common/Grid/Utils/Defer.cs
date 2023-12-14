using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils
{
    /// <summary>
    /// Love for MS (code copied from their quickgrid)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Defer : ComponentBase
    {
        /// <summary>
        /// For internal use only. Do not use.
        /// </summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// For internal use only. Do not use.
        /// </summary>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, ChildContent);
        }
    }
}
