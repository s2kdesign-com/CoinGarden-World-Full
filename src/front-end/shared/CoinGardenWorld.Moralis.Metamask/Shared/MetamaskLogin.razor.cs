using CoinGardenWorld.Moralis.Metamask.Exceptions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CoinGardenWorld.Moralis.Metamask.Shared
{
    public partial class MetamaskLogin : IDisposable
    {
        [Inject]
        public IMetaMaskService MetaMaskService { get; set; } = default!;

        public bool HasMetaMask { get; set; }
        public string? SelectedChain { get; set; }
        public string? TransactionCount { get; set; }

        public string Message { get; set; }
        public bool IsSiteConnected { get; set; }
        public string? EthAddress { get; set; }
        public string EthAddressShort { get; set; }
        public string EthBalance { get; set; }


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
            await GetSelectedAddress();
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
            await GetSelectedAddress();
            await GetBalance();
            StateHasChanged();
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

        public async Task GetSelectedNetwork()
        {
            var chainId = await MetaMaskService.GetSelectedChain();

            SelectedChain = $"ChainID: {chainId}";
            Console.WriteLine($"ChainID: {chainId}");
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
