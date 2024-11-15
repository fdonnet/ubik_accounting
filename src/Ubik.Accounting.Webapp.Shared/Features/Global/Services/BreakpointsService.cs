using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Webapp.Shared.Features.Global.Services
{
    public class BreakpointsService(IJSRuntime jsRuntime) : IAsyncDisposable
    {
        public bool IsSmallDevice { get; private set; } = false;
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private DotNetObjectReference<BreakpointsService>? _dotNetRef = null;

        public event Action OnDeviceChanged = default!;

        private static readonly string[] _allowedBreakpoints = ["2xl", "xl", "lg", "md", "sm", "xs"];

        public async Task InitializeAsync()
        {
            if (_dotNetRef == null)
            {
                _dotNetRef = DotNetObjectReference.Create(this);
                await _jsRuntime.InvokeVoidAsync("breakpointService.initialize", _dotNetRef);
            }
        }

        [JSInvokable]
        public void OnBreakpointChangedInClient(string breakpoint)
        {
            if(_allowedBreakpoints.Contains(breakpoint))
                ChangeDevice(breakpoint);
        }

        public async Task InitCurrentBreakPointAsync(string? breakpoint = null)
        {
            breakpoint ??= await GetCurrentBreakpointAsync();

            ChangeDevice(breakpoint);
        }

        private async Task<string> GetCurrentBreakpointAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("breakpointService.getCurrentBreakpoint");
        }

        private void ChangeDevice(string breakpoint)
        {
            var initialStatus = IsSmallDevice;

            IsSmallDevice = breakpoint switch
            {
                "xs" or "sm" or "md" => true,
                _ => false,
            };

            if (IsSmallDevice != initialStatus)
            {
                OnDeviceChanged?.Invoke();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _jsRuntime.InvokeVoidAsync("breakpointService.dispose");
            _dotNetRef?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
