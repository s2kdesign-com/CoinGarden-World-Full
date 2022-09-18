namespace CoinGardenWorld.Moralis.Metamask.Exceptions
{
    public class UserDeniedException : ApplicationException
    {
        public UserDeniedException() : base("User denied access to MetaMask.")
        {

        }
    }
}
