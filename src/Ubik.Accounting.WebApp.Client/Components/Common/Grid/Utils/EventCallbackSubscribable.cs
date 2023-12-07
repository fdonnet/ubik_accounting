using Microsoft.AspNetCore.Components;

namespace Ubik.Accounting.WebApp.Client.Components.Common.Grid.Utils
{
    internal sealed class EventCallbackSubscribable<T>
    {
        private readonly Dictionary<EventCallbackSubscriber<T>, EventCallback<T>> _callbacks = new();

        /// <summary>
        /// Invokes all the registered callbacks sequentially, in an undefined order.
        /// </summary>
        public async Task InvokeCallbacksAsync(T eventArg)
        {
            foreach (var callback in _callbacks.Values)
            {
                await callback.InvokeAsync(eventArg);
            }
        }

        // Don't call this directly - it gets called by EventCallbackSubscription
        public void Subscribe(EventCallbackSubscriber<T> owner, EventCallback<T> callback)
            => _callbacks.Add(owner, callback);

        // Don't call this directly - it gets called by EventCallbackSubscription
        public void Unsubscribe(EventCallbackSubscriber<T> owner)
            => _callbacks.Remove(owner);
    }
}
