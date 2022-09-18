namespace CoinGardenWorld.Moralis.Metamask.Exceptions
{
    public class NoMetaMaskException : ApplicationException
    {
        public NoMetaMaskException() : base("MetaMask is not installed.")
        {

        }
    }
}
