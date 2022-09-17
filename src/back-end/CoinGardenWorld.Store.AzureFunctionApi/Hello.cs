using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CoinGardenWorld.Store.AzureFunctionApi
{
    public class Hello
    {
        private readonly ILogger _logger;

        public Hello(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Hello>();
        }

        [Function("SayHello")]
        public HttpResponseData SayHello([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);

            response.WriteAsJsonAsync("Hello from CoinGarden.World NFT API - Azure Functions!");

            return response;
        }
    }
}
