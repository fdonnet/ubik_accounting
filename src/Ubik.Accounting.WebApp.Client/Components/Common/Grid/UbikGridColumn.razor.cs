//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Rendering;
//using System.Linq.Expressions;
//using Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils;

//namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid
//{
//    public partial class UbikGridColumn<TGridItem, TProp>
//    {
//        [CascadingParameter] internal InternalGridContext<TGridItem> InternalGridContext { get; set; } = default!;
//        //[CascadingParameter] public UbikGrid<TGridItem>? ParentUbikGrid { get; set; }
//        [Parameter] public string Label { get; set; } = "Label";
//        [Parameter, EditorRequired] public Expression<Func<TGridItem, TProp>> Property { get; set; } = default!;

//        private Expression<Func<TGridItem, TProp>>? _lastAssignedProperty;
//        private Func<TGridItem, string?>? _cellTextFunc;
//        [Parameter] public string? Format { get; set; }


//        protected void OnParametersSet()
//        {
//            // We have to do a bit of pre-processing on the lambda expression. Only do that if it's new or changed.
//            if (_lastAssignedProperty != Property)
//            {
//                _lastAssignedProperty = Property;
//                var compiledPropertyExpression = Property.Compile();

//                if (!string.IsNullOrEmpty(Format))
//                {
//                    // TODO: Consider using reflection to avoid having to box every value just to call IFormattable.ToString
//                    // For example, define a method "string Format<U>(Func<TGridItem, U> property) where U: IFormattable", and
//                    // then construct the closed type here with U=TProp when we know TProp implements IFormattable

//                    // If the type is nullable, we're interested in formatting the underlying type
//                    var nullableUnderlyingTypeOrNull = Nullable.GetUnderlyingType(typeof(TProp));
//                    if (!typeof(IFormattable).IsAssignableFrom(nullableUnderlyingTypeOrNull ?? typeof(TProp)))
//                    {
//                        throw new InvalidOperationException($"A '{nameof(Format)}' parameter was supplied, but the type '{typeof(TProp)}' does not implement '{typeof(IFormattable)}'.");
//                    }

//                    _cellTextFunc = item => ((IFormattable?)compiledPropertyExpression!(item))?.ToString(Format, null);
//                }
//                else
//                {
//                    _cellTextFunc = item => compiledPropertyExpression!(item)?.ToString();
//                }

//                //_sortBuilder = GridSort<TGridItem>.ByAscending(Property);
//            }

//            if (Label is null && Property.Body is MemberExpression memberExpression)
//            {
//                Label = memberExpression.Member.Name;
//            }
//        }

//        protected internal void CellContent(RenderTreeBuilder builder, TGridItem item)
//        => builder.AddContent(0, _cellTextFunc!(item));
//    }
//}
