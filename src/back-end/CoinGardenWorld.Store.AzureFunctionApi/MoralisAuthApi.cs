using System.Collections.Generic;
using System.Net;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moralis.AuthApi.Interfaces;
using Moralis.AuthApi.Models;
using Moralis.Network;
using Newtonsoft.Json;

namespace CoinGardenWorld.Store.AzureFunctionApi
{
    public class MoralisAuthApi
    {
        private readonly ILogger _logger;
        private static string AuthenticationApiUrl = Environment.GetEnvironmentVariable("MORALIS_AUTHENTICATION_API_URL", EnvironmentVariableTarget.Process);
        private static string Web3ApiUrl = Environment.GetEnvironmentVariable("MORALIS_WEB3_API_URL", EnvironmentVariableTarget.Process);
        private static string ApiKey = Environment.GetEnvironmentVariable("MORALIS_API_KEY", EnvironmentVariableTarget.Process);

        public MoralisAuthApi(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MoralisAuthApi>();
        }
        
        [Function("ChallengeRequest")]
        public async Task<HttpResponseData> ChallengeRequest([HttpTrigger(AuthorizationLevel.Function,  "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // Create the function execution's context through the request
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // Deserialize Playfab context
            dynamic context = JsonConvert.DeserializeObject(requestBody);
            var args = context.FunctionArgument;

            // Get the address from the request
            dynamic address = null;
            if (args != null && args["address"] != null)
            {
                address = args["address"];
            }

            // Get the chainid from the request
            dynamic chainid = null;
            if (args != null && args["chainid"] != null)
            {
                chainid = args["chainid"];
            }

            try
            {
                // Connect with the Moralis Authtication Server
                Moralis.AuthApi.MoralisAuthApiClient.Initialize(AuthenticationApiUrl, ApiKey);
                IAuthClientApi AuthenticationApi = Moralis.AuthApi.MoralisAuthApiClient.AuthenticationApi;

                // Create the authentication message and send it back to the client
                // Resources must be RFC 3986 URIs
                // More info here: https://eips.ethereum.org/EIPS/eip-4361#message-field-descriptions
                ChallengeRequestDto request = new ChallengeRequestDto()
                {
                    Address = address,
                    ChainId = (long)chainid,
                    Domain = "moralis.io",
                    ExpirationTime = DateTime.UtcNow.AddMinutes(60),
                    NotBefore = DateTime.UtcNow,
                    Resources = new string[] { "https://docs.moralis.io/" },
                    Timeout = 120,
                    Statement = "Please confirm",
                    Uri = "https://moralis.io/"
                };

                ChallengeResponseDto authResult = await AuthenticationApi.AuthEndpoint.Challenge(request, ChainNetworkType.evm);

                var response = req.CreateResponse(HttpStatusCode.OK);

                response.WriteAsJsonAsync(authResult.Message);

                return response;
            }
            catch (ApiException aexp)
            {
                _logger.LogDebug($"aexp.Message: {aexp.ToString()}");

                var response = req.CreateResponse(HttpStatusCode.InternalServerError);

                response.WriteAsJsonAsync(aexp.Message);

                return response;

            }
            catch (Exception exp)
            {
                _logger.LogDebug($"exp.Message: {exp.ToString()}");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);

                response.WriteAsJsonAsync(exp.Message);

                return response;
            }
        }
    }
}
