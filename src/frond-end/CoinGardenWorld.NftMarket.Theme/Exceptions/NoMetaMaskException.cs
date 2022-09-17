using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinGardenWorld.NftMarket.Theme.Exceptions
{
    public class NoMetaMaskException : ApplicationException
    {
        public NoMetaMaskException() : base("MetaMask is not installed.")
        {

        }
    }
}
