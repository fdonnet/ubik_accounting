using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
{
    [CascadingTypeParameter(nameof(TGridItem))]
    public partial class UbikGrid<TGridItem> 
    {
        [Parameter] public IQueryable<TGridItem>? Items { get; set; }
        [Parameter] public List<string> FieldNames { get; set; } = default!;
        [Parameter] public bool EditAndRemoveButton { get; set; } = false;
        [Parameter] public EventCallback<TGridItem> OnEditItem { get; set; }
        [Parameter] public EventCallback<TGridItem> OnDeleteItem { get; set; }
        [Parameter] public RenderFragment DataGridColumns { get; set; } = default!;
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;

        protected List<UbikGridColumn<TGridItem>> Columns { get; } = [];

        private readonly RenderFragment _renderLoading;
        private readonly RenderFragment _renderColumnHeaders;
        private readonly RenderFragment _renderRows;

        private bool _collectingColumns;
        private int _columnNumber = 0;

        public UbikGrid()
        {
            _renderLoading = RenderLoading;
            _renderColumnHeaders = RenderColumnHeaders;
            _renderRows = RenderRows;
        }

        public void AddColumn(UbikGridColumn<TGridItem> column)
        {
            Columns.Add(column);
        }

        //TODO Column creation based on QuickGrid to manage
        private void StartCollectingColumns()
        {
            //Columns.Clear();
            _collectingColumns = true;
        }

        private void FinishCollectingColumns()
        {
            _collectingColumns = false;
        }

        protected override Task OnInitializedAsync()
        {
            _columnNumber = EditAndRemoveButton ? FieldNames.Count + 2 : FieldNames.Count; 
            return base.OnInitializedAsync();
        }

        
        private async Task EditItem(TGridItem currentItem)
        {
            await OnEditItem.InvokeAsync(currentItem);
        }

        private async Task DeleteItem(TGridItem currentItem)
        {
            await OnDeleteItem.InvokeAsync(currentItem);
        }
    }
}
