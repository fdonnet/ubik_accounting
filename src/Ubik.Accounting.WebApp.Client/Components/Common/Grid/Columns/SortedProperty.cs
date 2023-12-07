namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Columns
{
    public readonly struct SortedProperty
    {
        /// <summary>
        /// The property name for the sorting rule.
        /// </summary>
        public required string PropertyName { get; init; }

        /// <summary>
        /// The direction to sort by.
        /// </summary>
        public SortDirection Direction { get; init; }
    }
}
