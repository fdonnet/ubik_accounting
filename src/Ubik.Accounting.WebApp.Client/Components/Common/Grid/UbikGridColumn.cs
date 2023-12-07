using Microsoft.AspNetCore.Components;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
{
    [CascadingTypeParameter(nameof(TGridItem))]
    public class UbikGridColumn<TGridItem> : ComponentBase
    {
        [CascadingParameter] public UbikGrid<TGridItem>? ParentUbikGrid { get; set; }
        [Parameter] public string Label { get; set; } = "Label";
        [Parameter] public string Field { get; set; } = default!;

        protected override void OnInitialized()
        {
            //base.OnInitialized();

            ParentUbikGrid?.AddColumn(this);

        }
    }
}
