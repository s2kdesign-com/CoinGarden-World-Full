using Microsoft.JSInterop;

namespace CoinGardenWorld.Web
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class ThemeJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public ThemeJsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./wwwroot/themeJsInterop.js").AsTask());
        }


        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}