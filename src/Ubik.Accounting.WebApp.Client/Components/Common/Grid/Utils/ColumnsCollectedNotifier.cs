using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils
{
    /// <summary>
    /// Love for MS (code copied from their quickgrid)
    /// </summary>
    /// <typeparam name="TGridItem"></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ColumnsCollectedNotifier<TGridItem> : Microsoft.AspNetCore.Components.IComponent
    {
        private bool _isFirstRender = true;

        [CascadingParameter] internal InternalGridContext<TGridItem> InternalGridContext { get; set; } = default!;

        public void Attach(RenderHandle renderHandle)
        {
            // This component never renders, so we can ignore the renderHandle
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            if (_isFirstRender)
            {
                _isFirstRender = false;
                parameters.SetParameterProperties(this);
                return InternalGridContext.ColumnsFirstCollected.InvokeCallbacksAsync(null);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
