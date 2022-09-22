using CoinGardenWorld.Moralis.Metamask.Exceptions;
using Microsoft.AspNetCore.Components;
using Moralis.AuthApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CoinGardenWorld.Moralis.Metamask.Extensions;
using CoinGardenWorld.Moralis.Metamask.Models;

namespace CoinGardenWorld.Moralis.Metamask.Shared
{
    public partial class MetamaskLogin : IDisposable
    {
        [Inject]
        public IMetaMaskService MetaMaskService { get; set; } = default!;
        [Inject]
        private HttpClient client { get; set; } = default!;

        public bool HasMetaMask { get; set; }
        public long? SelectedChain { get; set; }
        public string? TransactionCount { get; set; }

        public string Message { get; set; }
        public bool IsSiteConnected { get; set; }
        public bool IsMoralisVerified { get; set; }
        public string? EthAddress { get; set; }
        public string EthAddressShort { get; set; }
        public string EthBalance { get; set; }
        public string JwtToken { get; set; }


        protected override async Task OnInitializedAsync()
        {
            //Subscribe to events
            IMetaMaskService.AccountChangedEvent += MetaMaskService_AccountChangedEvent;
            IMetaMaskService.ChainChangedEvent += MetaMaskService_ChainChangedEvent;

            HasMetaMask = await MetaMaskService.HasMetaMask();
            if (HasMetaMask)
                await MetaMaskService.ListenToEvents();

            IsSiteConnected = await MetaMaskService.IsSiteConnected();
            if (IsSiteConnected)
            {
                await GetSelectedAddress();
                await GetSelectedNetwork();
                await GetBalance();
            }

        }
        private async Task LoginWithMetamask()
        {
            await MetaMaskService.ConnectMetaMask();
            var address = await GetSelectedAddress();
            var chain = await GetSelectedNetwork();

            if (!string.IsNullOrEmpty(EthAddress))
            {
                MoralisChallenge(address, chain);
            }
            StateHasChanged();
        }
        private async Task LogoutMetamask()
        {
            await MetaMaskService.Logout();
            StateHasChanged();
        }

        private async Task MetaMaskService_ChainChangedEvent(long arg)
        {
            await GetSelectedNetwork();
            StateHasChanged();
        }

        private async Task MetaMaskService_AccountChangedEvent(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                Console.WriteLine($"Account is locked");
                IsSiteConnected = true;
                EthAddress = null;
                EthAddressShort = null;
                StateHasChanged();
                return;
            }
            else
            {
                // TODO: Remove that 
                if (!string.IsNullOrEmpty(arg))
                {
                    MoralisChallenge(arg, SelectedChain ?? 0);
                }
            }
            await GetSelectedAddress();
            await GetBalance();
            StateHasChanged();
        }

        private async Task MoralisChallenge(string address, long chainId)
        {
            var chalengeResult = await client.PostAsJsonAsync("api/ChallengeRequest", new { address = address, chainid = chainId });
            if (chalengeResult.IsSuccessStatusCode)
            {
                var resultMessage =
                    JsonConvert.DeserializeObject<ChallengeResponseDto>(await chalengeResult.Content.ReadAsStringAsync());

                var data = new TypedDataPayload<Message>
                {
                    Domain = new Domain
                    {
                        Name = "CGW Store",
                        Version = "1",
                        ChainId = chainId
                    },
                    Types = new Dictionary<string, TypeMemberValue[]>
                    {
                        ["EIP712Domain"] = new[]
                        {
                                new TypeMemberValue { Name = "name", Type = "string" },
                                new TypeMemberValue { Name = "version", Type = "string" },
                                new TypeMemberValue { Name = "chainId", Type = "uint256" }
                            },
                        ["Message"] = new[]
                        {
                            new TypeMemberValue { Name = "contents", Type = "string" }
                        }
                    },
                    PrimaryType = "Message",
                    Message = new Message
                    {
                        contents = resultMessage.Message
                    }
                };
                var signature = await MetaMaskService.SignTypedDataV4(data.ToJson());

                // TODO: Remove
                Console.WriteLine(signature);

                var validationResult = await client.PostAsJsonAsync("api/VerifySignature", new
                {
                    message = resultMessage.Message,
                    signature
                });
                if (validationResult.IsSuccessStatusCode)
                {
                    JwtToken = await validationResult.Content.ReadAsStringAsync();
                    IsMoralisVerified = true;
                }
                else
                {

                    Console.WriteLine(await validationResult.Content.ReadAsStringAsync());
                }
            }
            else
            {

                Console.WriteLine(await chalengeResult.Content.ReadAsStringAsync());
            }

        }


        public async Task<string> GetSelectedAddress()
        {
            EthAddress = await MetaMaskService.GetSelectedAddress();
            if (!string.IsNullOrEmpty(EthAddress))
            {

                EthAddressShort = EthAddress.Substring(0, 6) + "..." + EthAddress.Substring(EthAddress.Length - 4, 4);
                Console.WriteLine($"Address: {EthAddress}");

            }
            return EthAddress;
        }

        public async Task<long> GetSelectedNetwork()
        {
            var chainId = await MetaMaskService.GetSelectedChain();

            SelectedChain = chainId;
            Console.WriteLine($"ChainID: {chainId}");

            return chainId;
        }

        public async Task GetTransactionCount()
        {
            var transactionCount = await MetaMaskService.GetTransactionCount();
            TransactionCount = $"Transaction count: {transactionCount}";
        }

        public async Task GetBalance()
        {
            var address = await MetaMaskService.GetSelectedAddress();
            var result = await MetaMaskService.GetBalance(address);
            EthBalance = $"ETH: {result} wei";
        }

        public void Dispose()
        {
            IMetaMaskService.AccountChangedEvent -= MetaMaskService_AccountChangedEvent;
            IMetaMaskService.ChainChangedEvent -= MetaMaskService_ChainChangedEvent;
        }
    }
}
