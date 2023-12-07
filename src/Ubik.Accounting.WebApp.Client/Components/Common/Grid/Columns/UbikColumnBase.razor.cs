using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Columns
{
    public abstract partial class UbikColumnBase<TGridItem>
    {
        [CascadingParameter] internal InternalGridContext<TGridItem> InternalGridContext { get; set; } = default!;
        [Parameter] public string? Title { get; set; }

        public UbikGrid<TGridItem> Grid => InternalGridContext.Grid;
        protected internal abstract void CellContent(RenderTreeBuilder builder, TGridItem item);
        protected internal RenderFragment HeaderContent { get; protected set; }

        public UbikColumnBase()
        {
            HeaderContent = RenderDefaultHeaderContent;
        }
    }
}
