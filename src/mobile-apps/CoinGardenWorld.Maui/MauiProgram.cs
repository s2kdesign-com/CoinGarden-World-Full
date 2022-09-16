using CoinGardenWorld.Maui.Authorization;
using CoinGardenWorld.Maui.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;

namespace CoinGardenWorld.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);

            // Add authorization (do not MSAL its not working with MAUI)
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();
            builder.Services.AddSingleton<ExternalAuthService>();
            //builder.Services.AddMsalAuthentication(options =>
            //{
            //    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
            //});

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif


            // Add CoinGardenWorld.MobileTheme GRPC, Pages and Components
            builder.Services.AddTheme();

            builder.Services.AddSingleton<WeatherForecastService>();

            return builder.Build();
        }
    }
}