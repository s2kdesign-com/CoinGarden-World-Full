using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinGardenWorld.NftMarket.Theme;
using Microsoft.JSInterop;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMetaMask(this IServiceCollection services)
        {

            services.AddScoped<IMetaMaskService>(sp => new MetaMaskService(sp.GetRequiredService<IJSRuntime>()));

            return services;
        }
    }

}