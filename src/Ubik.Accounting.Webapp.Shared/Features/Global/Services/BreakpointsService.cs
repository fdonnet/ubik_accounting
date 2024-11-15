using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Webapp.Shared.Features.Global.Services
{
    public class BreakpointsService(IJSRuntime jsRuntime)
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private DotNetObjectReference<BreakpointsService> _dotNetRef = default!;

        public event Action<string> OnBreakpointChanged = default!;

        public async Task InitializeAsync()
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("breakpointService.initialize", _dotNetRef);
        }

        [JSInvokable]
        public void OnBreakpointChangedInClient(string breakpoint)
        {
            OnBreakpointChanged?.Invoke(breakpoint);
        }
    }
}
