using System.Linq;
using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Columns;
namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils
{
    public readonly struct GridItemsProviderRequest<TGridItem>
    {
        /// <summary>
        /// The zero-based index of the first item to be supplied.
        /// </summary>
        public int StartIndex { get; init; }

        /// <summary>
        /// If set, the maximum number of items to be supplied. If not set, the maximum number is unlimited.
        /// </summary>
        public int? Count { get; init; }

        /// <summary>
        /// Specifies which column represents the sort order.
        ///
        /// Rather than inferring the sort rules manually, you should normally call either <see cref="ApplySorting(IQueryable{TGridItem})"/>
        /// or <see cref="GetSortByProperties"/>, since they also account for <see cref="SortByColumn" /> and <see cref="SortByAscending" /> automatically.
        /// </summary>
        public UbikColumnBase<TGridItem>? SortByColumn { get; init; }

        /// <summary>
        /// Specifies the current sort direction.
        ///
        /// Rather than inferring the sort rules manually, you should normally call either <see cref="ApplySorting(IQueryable{TGridItem})"/>
        /// or <see cref="GetSortByProperties"/>, since they also account for <see cref="SortByColumn" /> and <see cref="SortByAscending" /> automatically.
        /// </summary>
        public bool SortByAscending { get; init; }

        /// <summary>
        /// A token that indicates if the request should be cancelled.
        /// </summary>
        public CancellationToken CancellationToken { get; init; }

        internal GridItemsProviderRequest(
            int startIndex, int? count, UbikColumnBase<TGridItem>? sortByColumn, bool sortByAscending,
            CancellationToken cancellationToken)
        {
            StartIndex = startIndex;
            Count = count;
            SortByColumn = sortByColumn;
            SortByAscending = sortByAscending;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Applies the request's sorting rules to the supplied <see cref="IQueryable{TGridItem}"/>.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable{TGridItem}"/>.</param>
        /// <returns>A new <see cref="IQueryable{TGridItem}"/> representing the <paramref name="source"/> with sorting rules applied.</returns>
        public IQueryable<TGridItem> ApplySorting(IQueryable<TGridItem> source) =>
            SortByColumn?.SortBy?.Apply(source, SortByAscending) ?? source;

        /// <summary>
        /// Produces a collection of (property name, direction) pairs representing the sorting rules.
        /// </summary>
        /// <returns>A collection of (property name, direction) pairs representing the sorting rules</returns>
        public IReadOnlyCollection<SortedProperty> GetSortByProperties() =>
            SortByColumn?.SortBy?.ToPropertyList(SortByAscending) ?? Array.Empty<SortedProperty>();
    }
}
