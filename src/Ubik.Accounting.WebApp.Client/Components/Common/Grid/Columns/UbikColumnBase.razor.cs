using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Columns
{
    public abstract partial class UbikColumnBase<TGridItem>
    {
        //Params
        [CascadingParameter] internal InternalGridContext<TGridItem> InternalGridContext { get; set; } = default!;
        [Parameter] public string? Title { get; set; }
        [Parameter] public bool? Sortable { get; set; }
        [Parameter] public SortDirection InitialSortDirection { get; set; } = default;
        [Parameter] public bool IsDefaultSortColumn { get; set; } = false;

        public bool ShowSortIcon { get; set; } = true;


        //The context
        public UbikGrid<TGridItem> Grid => InternalGridContext.Grid;

        //Content
        protected internal RenderFragment HeaderContent { get; protected set; }
        protected internal abstract void CellContent(RenderTreeBuilder builder, TGridItem item);
        
        //Sort
        public abstract GridSort<TGridItem>? SortBy { get; set; }
        protected virtual bool IsSortableByDefault() => false;

        public UbikColumnBase()
        {
            HeaderContent = RenderDefaultHeaderContent;
        }

        protected override void OnInitialized()
        {
            if (IsDefaultSortColumn)
                ShowSortIcon = false;
        }
    }
}
