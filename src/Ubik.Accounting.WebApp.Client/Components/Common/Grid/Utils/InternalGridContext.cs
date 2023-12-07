namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils
{
    /// <summary>
    /// Love for MS (code copied from their quickgrid)
    /// </summary>
    internal sealed class InternalGridContext<TGridItem>
    {
        public UbikGrid<TGridItem> Grid { get; }
        public EventCallbackSubscribable<object?> ColumnsFirstCollected { get; } = new();

        public InternalGridContext(UbikGrid<TGridItem> grid)
        {
            Grid = grid;
        }
    }
}
