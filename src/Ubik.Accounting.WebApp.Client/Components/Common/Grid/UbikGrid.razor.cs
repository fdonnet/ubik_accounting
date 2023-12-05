using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
{
    public partial class UbikGrid<TGridItem>
    {
        [Parameter] public IQueryable<TGridItem>? Items { get; set; }
        [Parameter] public List<string> FieldNames { get; set; } = null;
        [Parameter] public bool EditAndRemoveButton { get; set; } = false;

        [Parameter]
        public EventCallback<TGridItem> OnEditItem { get; set; }
        [Parameter]
        public EventCallback<TGridItem> OnDeleteItem { get; set; }

        private readonly RenderFragment _renderLoading;
        private readonly RenderFragment _renderColumnHeaders;
        private readonly RenderFragment _renderRows;

        private int _columnNumber = 0;

        public UbikGrid()
        {
            _renderLoading = RenderLoading;
            _renderColumnHeaders = RenderColumnHeaders;
            _renderRows = RenderRows;
        }

        private async Task EditItem(MouseEventArgs e, TGridItem currentItem)
        {
            await OnEditItem.InvokeAsync(currentItem);
        }

        private async Task DeleteItem(MouseEventArgs e, TGridItem currentItem)
        {
            await OnDeleteItem.InvokeAsync(currentItem);
        }

        protected override Task OnInitializedAsync()
        {
            _columnNumber = EditAndRemoveButton ? FieldNames.Count + 2 : FieldNames.Count; 
            return base.OnInitializedAsync();
        }
    }
}
