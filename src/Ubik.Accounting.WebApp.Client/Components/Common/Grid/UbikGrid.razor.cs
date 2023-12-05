using Microsoft.AspNetCore.Components;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
{
    public partial class UbikGrid<TGridItem>
    {
        [Parameter] public IQueryable<TGridItem>? Items { get; set; }
        [Parameter] public List<string> FieldNames { get; set; } = null;
        [Parameter] public bool EditAndRemoveButton { get; set; } = false;

        private readonly RenderFragment _renderLoading;
        private readonly RenderFragment _renderColumnHeaders;
        private readonly RenderFragment _renderRows;
        private readonly RenderFragment _renderEditAndRemoveButtons;

        private int _columnNumber = 0;

        public UbikGrid()
        {
            _renderLoading = RenderLoading;
            _renderColumnHeaders = RenderColumnHeaders;
            _renderRows = RenderRows;
            _renderEditAndRemoveButtons = RenderEditRemoveButton;
        }

        protected override Task OnInitializedAsync()
        {
            _columnNumber = EditAndRemoveButton ? FieldNames.Count + 2 : FieldNames.Count; 
            return base.OnInitializedAsync();
        }
    }
}
