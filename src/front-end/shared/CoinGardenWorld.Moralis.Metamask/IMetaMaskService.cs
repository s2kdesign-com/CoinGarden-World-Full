using System.Numerics;
using CoinGardenWorld.Moralis.Metamask.Extensions;
using Microsoft.JSInterop;

namespace CoinGardenWorld.Moralis.Metamask;


public interface IMetaMaskService
{
    public static event Func<string, Task>? AccountChangedEvent;
    public static event Func<long,  Task>? ChainChangedEvent;

    ValueTask ConnectMetaMask();
    ValueTask DisposeAsync();
    ValueTask<dynamic> GenericRpc(string method, params dynamic[]? args);
    Task<BigInteger> GetBalance(string address, string block = "latest");
    ValueTask<string> GetSelectedAddress();
    ValueTask<long> GetSelectedChain();
    ValueTask<long> GetTransactionCount();
    ValueTask<bool> HasMetaMask();
    ValueTask<bool> IsSiteConnected();
    ValueTask ListenToEvents();
    ValueTask Logout();
    ValueTask<IJSObjectReference> LoadScripts(IJSRuntime jsRuntime);
    Task<string> RequestAccounts();
    ValueTask<string> SendTransaction(string to, BigInteger weiValue, string? data = null);

    [JSInvokable()]
    static async Task OnAccountsChanged(string selectedAccount)
    {
        if (AccountChangedEvent != null)
        {
            await AccountChangedEvent.Invoke(selectedAccount);
        }
    }

    [JSInvokable()]
    static async Task OnChainChanged(string chainhex)
    {
        if (ChainChangedEvent != null)
        {
            await ChainChangedEvent.Invoke(ChainHexToChainResponse(chainhex));
        }
    }

    protected static long ChainHexToChainResponse(string chainHex)
    {
        long chainId = chainHex.HexToInt();
        return chainId;
    }
}