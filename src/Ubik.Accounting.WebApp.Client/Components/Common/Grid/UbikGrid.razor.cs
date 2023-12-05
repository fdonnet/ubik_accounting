using Microsoft.AspNetCore.Components;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
{
    public partial class UbikGrid<TGridItem>
    {
        [Parameter] public IQueryable<TGridItem>? Items { get; set; }
        [Parameter] public List<string>? FieldNames { get; set; } = null;
    }
}
