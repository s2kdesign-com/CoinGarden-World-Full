using CoinGardenWorld.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace CoinGardenWorld.Grpc.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }
       // [Authorize]
       // TODO: Fix authorization (not much needed beacause the GRPC service is running without internet and only accesible from blazor )
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}