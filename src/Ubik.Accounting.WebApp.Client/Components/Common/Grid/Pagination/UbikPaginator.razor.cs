﻿using Microsoft.AspNetCore.Components;
using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Pagination
{
    public partial class UbikPaginator : IDisposable
    {
        private readonly EventCallbackSubscriber<PaginationState> _totalItemCountChanged;

        /// <summary>
        /// Specifies the associated <see cref="PaginationState"/>. This parameter is required.
        /// </summary>
        [Parameter, EditorRequired] public PaginationState State { get; set; } = default!;

        /// <summary>
        /// Optionally supplies a template for rendering the page count summary.
        /// </summary>
        [Parameter] public RenderFragment? SummaryTemplate { get; set; }

        /// <summary>
        /// Constructs an instance of <see cref="Paginator" />.
        /// </summary>
        public UbikPaginator()
        {
            // The "total item count" handler doesn't need to do anything except cause this component to re-render
            _totalItemCountChanged = new(new EventCallback<PaginationState>(this, null));
        }

        private Task GoFirstAsync() => GoToPageAsync(0);
        private Task GoPreviousAsync() => GoToPageAsync(State.CurrentPageIndex - 1);
        private Task GoNextAsync() => GoToPageAsync(State.CurrentPageIndex + 1);
        private Task GoLastAsync() => GoToPageAsync(State.LastPageIndex.GetValueOrDefault(0));

        private bool CanGoBack => State.CurrentPageIndex > 0;
        private bool CanGoForwards => State.CurrentPageIndex < State.LastPageIndex;

        private Task GoToPageAsync(int pageIndex)
            => State.SetCurrentPageIndexAsync(pageIndex);

        /// <inheritdoc />
        protected override void OnParametersSet()
            => _totalItemCountChanged.SubscribeOrMove(State.TotalItemCountChangedSubscribable);

        /// <inheritdoc />
        public void Dispose()
            => _totalItemCountChanged.Dispose();
    }
}
