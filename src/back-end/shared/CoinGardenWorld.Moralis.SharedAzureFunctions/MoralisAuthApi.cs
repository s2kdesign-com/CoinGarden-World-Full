using System.Collections.Generic;
using System.Net;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moralis.AuthApi.Interfaces;
using Moralis.AuthApi.Models;
using Moralis.Network;
using Moralis.Web3Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoinGardenWorld.MoralisShared.AzureFunctions
{
	public class MoralisAuthApi
	{
		private readonly ILogger _logger;

		private static string AuthenticationApiUrl =
			Environment.GetEnvironmentVariable("MORALIS_AUTHENTICATION_API_URL", EnvironmentVariableTarget.Process);

		private static string Web3ApiUrl =
			Environment.GetEnvironmentVariable("MORALIS_WEB3_API_URL", EnvironmentVariableTarget.Process);

		private static string ApiKey =
			Environment.GetEnvironmentVariable("MORALIS_API_KEY", EnvironmentVariableTarget.Process);

		public MoralisAuthApi(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<MoralisAuthApi>();
		}

		[Function("ChallengeRequest")]
		public async Task<HttpResponseData> ChallengeRequest(
			[HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");
			// Create the function execution's context through the request
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var challengeParams= JsonConvert.DeserializeObject<ChallengeRequestDto>(requestBody);

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
                    // The Ethereum address performing the signing conformant to capitalization encoded
                    // checksum specified in EIP-55 where applicable.
                     Address = challengeParams.Address,
                    // The EIP-155 Chain ID to which the session is bound, and the network where Contract
                    // Accounts MUST be resolved.
                    ChainId = (long)challengeParams.ChainId,
                    // The RFC 3986 authority that is requesting the signing
                    Domain = "coingarden.world",
                    // The ISO 8601 datetime string that, if present, indicates when the signed
                    // authentication message is no longer valid.
                    ExpirationTime = DateTime.UtcNow.AddDays(1),
                    // The ISO 8601 datetime string that, if present, indicates when the signed
                    // authentication message will become valid.
                    NotBefore = DateTime.UtcNow.AddMinutes(-1),
                    // A list of information or references to information the user wishes to have resolved
                    // as part of authentication by the relying party. They are expressed as RFC 3986 URIs
                    // separated by "\n- " where \n is the byte 0x0a.
                    Resources = new string[] { "https://coingarden.world" },
                    // Time is seconds at which point this request becomes invalid.
                    Timeout = 120,
                    // A human-readable ASCII assertion that the user will sign, and it must not
                    // contain '\n' (the byte 0x0a).
                    Statement = "Please confirm",
                    // An RFC 3986 URI referring to the resource that is the subject of the signing
                    // (as in the subject of a claim).
                    Uri = "https://coingarden.world"
                };

				ChallengeResponseDto authResult =
					await AuthenticationApi.AuthEndpoint.Challenge(request, ChainNetworkType.evm);

				var response = req.CreateResponse(HttpStatusCode.OK);

				await response.WriteAsJsonAsync(authResult);

				return response;
			}
			catch (ApiException aexp)
			{
				_logger.LogDebug($"aexp.Message: {aexp.ToString()}");
                throw;

            }
			catch (Exception exp)
			{
				_logger.LogDebug($"exp.Message: {exp.ToString()}");
				throw;

			}
		}


		[Function("VerifySignature")]
		public async Task<HttpResponseData> VerifySignature(
			[HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
		{
            var completedChallengeParams = await req.ReadFromJsonAsync<CompleteChallengeRequestDto>();

            CompleteChallengeResponseDto authResult = null;

			try
            {
				// Connect with the Moralis Authtication Server
				Moralis.AuthApi.MoralisAuthApiClient.Initialize(AuthenticationApiUrl, ApiKey);

                IAuthClientApi AuthenticationApi = Moralis.AuthApi.MoralisAuthApiClient.AuthenticationApi;
				
				authResult = await AuthenticationApi.AuthEndpoint.CompleteChallenge(completedChallengeParams, ChainNetworkType.evm);

                // ---------------------------------------------------------------------------------
                // Here is where you would save authentication information to the database.
                // ---------------------------------------------------------------------------------

                Dictionary<string, string> claims = new Dictionary<string, string>();
                claims.Add("Address", authResult.Address);
                claims.Add("AuthenticationProfileId", authResult.ProfileId);
                claims.Add("SignatureValidated", "true");

                var response = req.CreateResponse(HttpStatusCode.OK);

				await response.WriteAsJsonAsync(authResult);
				return response;
			}
			catch (ApiException aexp)
			{
				_logger.LogDebug($"aexp.Message: {aexp.ToString()}");

				throw aexp;
			}
			catch (Exception exp)
			{
				_logger.LogDebug($"exp.Message: {exp.ToString()}");
                throw exp;

			}
			// TODO: This is call to PlayFab to sync game profiles 
			//try
			//{
			//	// Get the setting from our Azure config and connect with the PlayFabAPI
			//	var settings = new PlayFabApiSettings
			//	{
			//		TitleId = Environment.GetEnvironmentVariable("PLAYFAB_TITLE_ID", EnvironmentVariableTarget.Process),
			//		DeveloperSecretKey = Environment.GetEnvironmentVariable("PLAYFAB_DEV_SECRET_KEY", EnvironmentVariableTarget.Process)
			//	};

			//	var serverApi = new PlayFabServerInstanceAPI(settings);

			//	// Update the user read-only data with the validated data and return the reponse to the client
			//	// Read-only data is data that the server can modify, but the client can only read
			//	var updateUserDataRequest = new UpdateUserDataRequest
			//	{
			//		PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
			//		Data = new Dictionary<string, string>()
			//		{
			//			{"MoralisProfileId", response.Id.ToString()},
			//			{"Address", response.Address.ToString()},
			//			{"ChainId", response.ChainId.ToString()}
			//		}
			//	};

			//	PlayFabResult<UpdateUserDataResult> updateUserDateResult = await serverApi.UpdateUserReadOnlyDataAsync(updateUserDataRequest);

			//	if (updateUserDateResult.Error == null)
			//	{
			//		return new OkObjectResult(updateUserDateResult.Result);
			//	}
			//	else
			//	{
			//		_logger.LogDebug($"updateUserDateResult.Error.ErrorMessage: {updateUserDateResult.Error.ErrorMessage.ToString()}");
			//		var response = req.CreateResponse(HttpStatusCode.InternalServerError);

			//		response.WriteAsJsonAsync(updateUserDateResult.Error.ErrorMessage);

			//		return response;
			//	}
			//}
			//catch (Exception exp)
			//{
			//	_logger.LogDebug($"exp.Message: {exp.ToString()}");
			//	var response = req.CreateResponse(HttpStatusCode.InternalServerError);

			//	response.WriteAsJsonAsync(exp.Message);

			//	return response;
			//}
		}
	}
    public static class JsonExtensions
    {
        public static string ToJson(this object model)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                    {
                        ProcessDictionaryKeys = false
                    }
                },

                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(model, serializerSettings);
        }
    }
}