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

				ChallengeResponseDto authResult =
					await AuthenticationApi.AuthEndpoint.Challenge(request, ChainNetworkType.evm);

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


		[Function("ChallengeVerify")]
		public async Task<HttpResponseData> ChallengeVerify(
			[HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
		{
			// Create the function execution's context through the request
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			// Deserialize Playfab context
			dynamic context = JsonConvert.DeserializeObject(requestBody);
			var args = context.FunctionArgument;

			// Get the message from the request
			dynamic message = null;
			if (args != null && args["message"] != null)
			{
				message = args["message"];
			}

			// Get the signature from the request
			dynamic signature = null;
			if (args != null && args["signature"] != null)
			{
				signature = args["signature"];
			}

			CompleteChallengeResponseDto authResult = null;

			try
			{
				// Connect with the Moralis Authtication Server
				Moralis.AuthApi.MoralisAuthApiClient.Initialize(AuthenticationApiUrl, ApiKey);
				IAuthClientApi AuthenticationApi = Moralis.AuthApi.MoralisAuthApiClient.AuthenticationApi;

				// Create the authentication message and send it back to the client
				CompleteChallengeRequestDto request = new CompleteChallengeRequestDto()
				{
					Message = message,
					Signature = signature
				};

				authResult = await AuthenticationApi.AuthEndpoint.CompleteChallenge(request, ChainNetworkType.evm);

				var response = req.CreateResponse(HttpStatusCode.OK);

				response.WriteAsJsonAsync(authResult);
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
}