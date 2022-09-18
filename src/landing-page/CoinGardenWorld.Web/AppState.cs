using Microsoft.JSInterop;

namespace CoinGardenWorld.Web
{
    public class AppState
    {
        public event Action AfterPageRendered;
        public static IJSObjectReference ThemeModule;
        public void PageRendered(bool firstRender)
        {
            AfterPageRendered?.Invoke();
        }
    }
}
