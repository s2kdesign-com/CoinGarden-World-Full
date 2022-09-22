using CoinGardenWorld.NftMarket;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
#if DEBUG
baseAddress = new Uri("http://localhost:7182");
#endif

// Defined in CoinGardenWorld.NftMarket.Theme
builder.Services.AddMetaMask(baseAddress);

await builder.Build().RunAsync();
