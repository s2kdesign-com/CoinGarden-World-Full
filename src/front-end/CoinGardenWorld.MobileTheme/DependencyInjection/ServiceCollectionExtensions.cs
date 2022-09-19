using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTheme(this IServiceCollection services)
        {

            services.AddSingleton(services =>
            {
                var baseUri = "https://localhost:7142/";
                var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpHandler = new GrpcWebHandler(new HttpClientHandler()) });
                return new CoinGardenWorld.MobileApps.Grpc.Greeter.GreeterClient(channel);
            });

            return services;
        }
    }
}