using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Columns
{
    public class UbikTemplateColumnTemplateColumn<TGridItem> : UbikColumnBase<TGridItem>
    {
        private static readonly RenderFragment<TGridItem> EmptyChildContent = _ => builder => { };

        /// <summary>
        /// Specifies the content to be rendered for each row in the table.
        /// </summary>
        [Parameter] public RenderFragment<TGridItem> ChildContent { get; set; } = EmptyChildContent;

        /// <inheritdoc/>
        [Parameter] public override GridSort<TGridItem>? SortBy { get; set; }

        /// <inheritdoc />
        protected internal override void CellContent(RenderTreeBuilder builder, TGridItem item)
            => builder.AddContent(0, ChildContent(item));

        /// <inheritdoc />
        protected override bool IsSortableByDefault()
            => SortBy is not null;
    }
}
