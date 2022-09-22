using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinGardenWorld.Moralis.Metamask;
using Microsoft.JSInterop;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMetaMask(this IServiceCollection services, Uri baseAddress)
        {

            services.AddScoped<IMetaMaskService>(sp => new MetaMaskService(sp.GetRequiredService<IJSRuntime>()));

            services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });

            return services;
        }
    }

}