using CoinGardenWorld.NftMarket;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#if !DEBUG
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
#else 
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:7182") });
#endif

// Defined in CoinGardenWorld.NftMarket.Theme
builder.Services.AddMetaMask();

await builder.Build().RunAsync();
