using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Linq.Expressions;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Columns
{
    public class UbikColumnProperty<TGridItem, TProp> : UbikColumnBase<TGridItem>
    {
        //Value in column
        [Parameter, EditorRequired] public Expression<Func<TGridItem, TProp>> Property { get; set; } = default!;
        //Format
        [Parameter] public string? Format { get; set; }

        //Manage a property linked col
        private Expression<Func<TGridItem, TProp>>? _lastAssignedProperty;
        private Func<TGridItem, string?>? _cellTextFunc;
        private GridSort<TGridItem>? _sortBuilder;

        public override GridSort<TGridItem>? SortBy
        {
            get => _sortBuilder;
            //set => throw new NotSupportedException($"PropertyColumn generates this member internally. For custom sorting rules, see '{typeof(TemplateColumn<TGridItem>)}'.");
            set => throw new NotSupportedException($"PropertyColumn generates this member internally. For custom sorting rules, see 'typeof(TemplateColumn<TGridItem>'.");
        }

        protected override void OnParametersSet()
        {
            // We have to do a bit of pre-processing on the lambda expression. Only do that if it's new or changed.
            if (_lastAssignedProperty != Property)
            {
                _lastAssignedProperty = Property;
                var compiledPropertyExpression = Property.Compile();

                if (!string.IsNullOrEmpty(Format))
                {
                    // TODO: Consider using reflection to avoid having to box every value just to call IFormattable.ToString
                    // For example, define a method "string Format<U>(Func<TGridItem, U> property) where U: IFormattable", and
                    // then construct the closed type here with U=TProp when we know TProp implements IFormattable

                    // If the type is nullable, we're interested in formatting the underlying type
                    var nullableUnderlyingTypeOrNull = Nullable.GetUnderlyingType(typeof(TProp));
                    if (!typeof(IFormattable).IsAssignableFrom(nullableUnderlyingTypeOrNull ?? typeof(TProp)))
                    {
                        throw new InvalidOperationException($"A '{nameof(Format)}' parameter was supplied, but the type '{typeof(TProp)}' does not implement '{typeof(IFormattable)}'.");
                    }

                    _cellTextFunc = item => ((IFormattable?)compiledPropertyExpression!(item))?.ToString(Format, null);
                }
                else
                {
                    _cellTextFunc = item => compiledPropertyExpression!(item)?.ToString();
                }

                _sortBuilder = GridSort<TGridItem>.ByAscending(Property);
            }

            if (Title is null && Property.Body is MemberExpression memberExpression)
            {
                Title = memberExpression.Member.Name;
            }
        }

        /// <inheritdoc />
        protected internal override void CellContent(RenderTreeBuilder builder, TGridItem item)
            => builder.AddContent(0, _cellTextFunc!(item));
    }
}
