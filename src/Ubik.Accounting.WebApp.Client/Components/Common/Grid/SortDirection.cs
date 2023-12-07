namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
{
    // The .NET Foundation licenses this file to you under the MIT license.
    public enum SortDirection
    {
        /// <summary>
        /// Automatic sort order. When used with <see cref="QuickGrid{TGridItem}.SortByColumnAsync(ColumnBase{TGridItem}, SortDirection)"/>,
        /// the sort order will automatically toggle between <see cref="Ascending"/> and <see cref="Descending"/> on successive calls, and
        /// resets to <see cref="Ascending"/> whenever the specified column is changed.
        /// </summary>
        Auto,

        /// <summary>
        /// Ascending order.
        /// </summary>
        Ascending,

        /// <summary>
        /// Descending order.
        /// </summary>
        Descending,
    }
}
