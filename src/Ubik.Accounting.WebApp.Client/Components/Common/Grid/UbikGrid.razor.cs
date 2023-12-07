using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Columns;
using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
{
    [CascadingTypeParameter(nameof(TGridItem))]
    public partial class UbikGrid<TGridItem>
    {
        [Parameter] public IQueryable<TGridItem>? Items { get; set; }
        //[Parameter] public List<string> FieldNames { get; set; } = default!;
        [Parameter] public bool EditAndRemoveButton { get; set; } = false;
        [Parameter] public EventCallback<TGridItem> OnEditItem { get; set; }
        [Parameter] public EventCallback<TGridItem> OnDeleteItem { get; set; }
        [Parameter] public RenderFragment DataGridColumns { get; set; } = default!;
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;

        private readonly InternalGridContext<TGridItem> _internalGridContext;
        private readonly List<UbikColumnBase<TGridItem>> _columns;
        private ICollection<TGridItem> _currentNonVirtualizedViewItems = Array.Empty<TGridItem>();

        private readonly RenderFragment _renderLoading;
        private readonly RenderFragment _renderColumnHeaders;
        private readonly RenderFragment _renderNonVirtualizedRows;
        //private readonly RenderFragment _renderRows;

        private bool _collectingColumns;
        private int _columnNumber = 99;

        public UbikGrid()
        {
            _columns = new();
            _internalGridContext = new(this);

            _renderLoading = RenderLoading;
            _renderColumnHeaders = RenderColumnHeaders;
            _renderNonVirtualizedRows = RenderNonVirtualizedRows;
            //_renderRows = RenderRows;

            var columnsFirstCollectedSubscriber = new EventCallbackSubscriber<object?>(
            EventCallback.Factory.Create<object?>(this, RefreshDataCoreAsync));
            columnsFirstCollectedSubscriber.SubscribeOrMove(_internalGridContext.ColumnsFirstCollected);
        }

        protected override Task OnParametersSetAsync()
        {
            //// The associated pagination state may have been added/removed/replaced
            //_currentPageItemsChanged.SubscribeOrMove(Pagination?.CurrentPageItemsChanged);

            //if (Items is not null && ItemsProvider is not null)
            //{
            //    throw new InvalidOperationException($"{nameof(QuickGrid)} requires one of {nameof(Items)} or {nameof(ItemsProvider)}, but both were specified.");
            //}

            //// Perform a re-query only if the data source or something else has changed
            //var _newItemsOrItemsProvider = Items ?? (object?)ItemsProvider;
            //var dataSourceHasChanged = _newItemsOrItemsProvider != _lastAssignedItemsOrProvider;
            //if (dataSourceHasChanged)
            //{
            //    _lastAssignedItemsOrProvider = _newItemsOrItemsProvider;
            //    _asyncQueryExecutor = AsyncQueryExecutorSupplier.GetAsyncQueryExecutor(Services, Items);
            //}

            //var mustRefreshData = dataSourceHasChanged
            //    || (Pagination?.GetHashCode() != _lastRefreshedPaginationStateHash);

            // We don't want to trigger the first data load until we've collected the initial set of columns,
            // because they might perform some action like setting the default sort order, so it would be wasteful
            // to have to re-query immediately

            _currentNonVirtualizedViewItems = Items?.ToArray() ?? [];

            return (_columns.Count > 0) ? RefreshDataCoreAsync() : Task.CompletedTask;
        }

        private Task RefreshDataCoreAsync()
        {
            //_currentNonVirtualizedViewItems = Items;
            return Task.CompletedTask;
            //var result = await ResolveItemsRequestAsync(request);
        }

        public void AddColumn(UbikColumnBase<TGridItem> column)
        {
            if(_collectingColumns)
            {
                _columns.Add(column);
            }
        }

        //TODO Column creation based on QuickGrid to manage
        private void StartCollectingColumns()
        {
            _columns.Clear();
            _collectingColumns = true;
        }

        private void FinishCollectingColumns()
        {
            _columnNumber = EditAndRemoveButton ? _columns.Count + 2 : _columns.Count;
            _collectingColumns = false;
        }

        //protected override Task OnInitializedAsync()
        //{
        //    _columnNumber = EditAndRemoveButton ? FieldNames.Count + 2 : FieldNames.Count;
        //    return base.OnInitializedAsync();
        //}


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
