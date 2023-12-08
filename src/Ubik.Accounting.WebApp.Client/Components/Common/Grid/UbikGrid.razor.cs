//A lot of things have been take from Microsoft --- Thx for your quick grid guys !!!
//Need to continue to transfer what I need (virtualize, sorting, filtering etc)
//TODO: don't forget to check their sources sometimes to retrieve the new of foing thing.
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Columns;
using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
{
    [CascadingTypeParameter(nameof(TGridItem))]
    public partial class UbikGrid<TGridItem>
    {
        [Inject] private IServiceProvider Services { get; set; } = default!;

        [Parameter] public IQueryable<TGridItem>? Items { get; set; }
        [Parameter] public GridItemsProvider<TGridItem>? ItemsProvider { get; set; }
        [Parameter] public RenderFragment DataGridColumns { get; set; } = default!;
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;
        [Parameter] public bool HighlightFirstColumn { get; set; } = false;

        private readonly InternalGridContext<TGridItem> _internalGridContext;
        private readonly List<UbikColumnBase<TGridItem>> _columns;
        private ICollection<TGridItem> _currentNonVirtualizedViewItems = Array.Empty<TGridItem>();

        private readonly RenderFragment _renderLoading;
        private readonly RenderFragment _renderColumnHeaders;
        private readonly RenderFragment _renderNonVirtualizedRows;

        private bool _collectingColumns;
        private int _columnNumber = 99;
        private int _ariaBodyRowCount;

        // IQueryable only exposes synchronous query APIs. IAsyncQueryExecutor is an adapter that lets us invoke any
        // async query APIs that might be available. We have built-in support for using EF Core's async query APIs.
        private IAsyncQueryExecutor? _asyncQueryExecutor;

        //If need a refreash from outside
        private int? _lastRefreshedPaginationStateHash;
        private object? _lastAssignedItemsOrProvider;
        private CancellationTokenSource? _pendingDataLoadCancellationTokenSource;

        //Sort
        private UbikColumnBase<TGridItem>? _displayOptionsForColumn;
        public UbikColumnBase<TGridItem>? SortByColumn { get; private set; }
        public bool SortByAscending { get; private set; }
        private bool _checkColumnOptionsPosition;

        public UbikGrid()
        {
            _columns = [];
            _internalGridContext = new(this);

            _renderLoading = RenderLoading;
            _renderColumnHeaders = RenderColumnHeaders;
            _renderNonVirtualizedRows = RenderNonVirtualizedRows;

            var columnsFirstCollectedSubscriber = new EventCallbackSubscriber<object?>(
            EventCallback.Factory.Create<object?>(this, RefreshDataCoreAsync));
            columnsFirstCollectedSubscriber.SubscribeOrMove(_internalGridContext.ColumnsFirstCollected);
        }

        protected override Task OnParametersSetAsync()
        {
            //// The associated pagination state may have been added/removed/replaced
            //_currentPageItemsChanged.SubscribeOrMove(Pagination?.CurrentPageItemsChanged);

            if (Items is not null && ItemsProvider is not null)
            {
                throw new InvalidOperationException($"UbikGrid requires one of {nameof(Items)} or {nameof(ItemsProvider)}, but both were specified.");
            }

            // Perform a re-query only if the data source or something else has changed
            var _newItemsOrItemsProvider = Items ?? (object?)ItemsProvider;
            var dataSourceHasChanged = _newItemsOrItemsProvider != _lastAssignedItemsOrProvider;
            if (dataSourceHasChanged)
            {
                _lastAssignedItemsOrProvider = _newItemsOrItemsProvider;
                _asyncQueryExecutor = AsyncQueryExecutorSupplier.GetAsyncQueryExecutor(Services, Items);
            }

            //var mustRefreshData = dataSourceHasChanged
            //    || (Pagination?.GetHashCode() != _lastRefreshedPaginationStateHash);
            var mustRefreshData = dataSourceHasChanged;

            // We don't want to trigger the first data load until we've collected the initial set of columns,
            // because they might perform some action like setting the default sort order, so it would be wasteful
            // to have to re-query immediately

            return (_columns.Count > 0 && mustRefreshData) ? RefreshDataCoreAsync() : Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //if (firstRender)
            //{
            //    _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Microsoft.AspNetCore.Components.QuickGrid/QuickGrid.razor.js");
            //    _jsEventDisposable = await _jsModule.InvokeAsync<IJSObjectReference>("init", _tableReference);
            //}

            //if (_checkColumnOptionsPosition && _displayOptionsForColumn is not null)
            //{
            //    _checkColumnOptionsPosition = false;
            //    _ = _jsModule?.InvokeVoidAsync("checkColumnOptionsPosition", _tableReference).AsTask();
            //}
        }

        public void AddColumn(UbikColumnBase<TGridItem> column, SortDirection? initialSortDirection, bool isDefaultSortColumn)
        {
            if (_collectingColumns)
            {
                _columns.Add(column);

                if (isDefaultSortColumn && SortByColumn is null && initialSortDirection.HasValue)
                {
                    SortByColumn = column;
                    SortByAscending = initialSortDirection.Value != SortDirection.Descending;
                }
            }
        }

        private void StartCollectingColumns()
        {
            _columns.Clear();
            _collectingColumns = true;
        }

        private void FinishCollectingColumns()
        {
            _collectingColumns = false;
        }

        public Task ShowColumnOptionsAsync(UbikColumnBase<TGridItem> column)
        {
            _displayOptionsForColumn = column;
            //_checkColumnOptionsPosition = true; // Triggers a call to JS to position the options element, apply autofocus, and any other setup
            StateHasChanged();
            return Task.CompletedTask;
        }

        //Call a refresh
        public async Task RefreshDataAsync()
        {
            await RefreshDataCoreAsync();
            StateHasChanged();
        }

        public Task SortByColumnAsync(UbikColumnBase<TGridItem> column, SortDirection direction = SortDirection.Auto)
        {
            if(column.IsDefaultSortColumn)
                column.ShowSortIcon = true;

            SortByAscending = direction switch
            {
                SortDirection.Ascending => true,
                SortDirection.Descending => false,
                SortDirection.Auto => SortByColumn != column || !SortByAscending,
                _ => throw new NotSupportedException($"Unknown sort direction {direction}"),
            };

            SortByColumn = column;

            StateHasChanged(); // We want to see the updated sort order in the header, even before the data query is completed
            return RefreshDataAsync();
        }

        private async Task RefreshDataCoreAsync()
        {
            // Move into a "loading" state, cancelling any earlier-but-still-pending load
            _pendingDataLoadCancellationTokenSource?.Cancel();
            var thisLoadCts = _pendingDataLoadCancellationTokenSource = new CancellationTokenSource();

            //if (_virtualizeComponent is not null)
            //{
            //    // If we're using Virtualize, we have to go through its RefreshDataAsync API otherwise:
            //    // (1) It won't know to update its own internal state if the provider output has changed
            //    // (2) We won't know what slice of data to query for
            //    await _virtualizeComponent.RefreshDataAsync();
            //    _pendingDataLoadCancellationTokenSource = null;
            //}
            //else
            //{
            // If we're not using Virtualize, we build and execute a request against the items provider directly
            //_lastRefreshedPaginationStateHash = Pagination?.GetHashCode();
            //var startIndex = Pagination is null ? 0 : (Pagination.CurrentPageIndex * Pagination.ItemsPerPage);
            //var request = new GridItemsProviderRequest<TGridItem>(
            //    startIndex, Pagination?.ItemsPerPage, _sortByColumn, _sortByAscending, thisLoadCts.Token);

            var request = new GridItemsProviderRequest<TGridItem>(
                    0, null, SortByColumn, SortByAscending, thisLoadCts.Token);

            var result = await ResolveItemsRequestAsync(request);
                if (!thisLoadCts.IsCancellationRequested)
                {
                    _currentNonVirtualizedViewItems = result.Items;
                    _ariaBodyRowCount = _currentNonVirtualizedViewItems.Count;
                    //Pagination?.SetTotalItemCountAsync(result.TotalItemCount);
                    _pendingDataLoadCancellationTokenSource = null;
                }
            //}
        }

        // Normalizes all the different ways of configuring a data source so they have common GridItemsProvider-shaped API
        //Will be used when we will virtualize
        private async ValueTask<GridItemsProviderResult<TGridItem>> ResolveItemsRequestAsync(GridItemsProviderRequest<TGridItem> request)
        {
            if (ItemsProvider is not null)
            {
                return await ItemsProvider(request);
            }
            else if (Items is not null)
            {
                var totalItemCount = _asyncQueryExecutor is null ? Items.Count() : await _asyncQueryExecutor.CountAsync(Items);
                var result = request.ApplySorting(Items).Skip(request.StartIndex);
                if (request.Count.HasValue)
                {
                    result = result.Take(request.Count.Value);
                }
                var resultArray = _asyncQueryExecutor is null ? result.ToArray() : await _asyncQueryExecutor.ToArrayAsync(result);
                return GridItemsProviderResult.From(resultArray, totalItemCount);
            }
            else
            {
                return GridItemsProviderResult.From(Array.Empty<TGridItem>(), 0);
            }
        }

        private string AriaSortValue(UbikColumnBase<TGridItem> column)
            => SortByColumn == column
                ? (SortByAscending ? "ascending" : "descending")
                : "none";
    }
}
