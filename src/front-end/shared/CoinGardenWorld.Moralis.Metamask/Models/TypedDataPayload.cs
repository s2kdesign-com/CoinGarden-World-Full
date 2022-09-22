using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CoinGardenWorld.Moralis.Metamask.Models
{
    public record struct Message(string contents);


    public class TypedDataPayload<T>
    {
        public Dictionary<string, TypeMemberValue[]> Types { get; set; } = new();
        public string? PrimaryType { get; set; }
        public Domain? Domain { get; set; }
        public T? Message { get; set; }


    }

    public class Domain
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
        public BigInteger? ChainId { get; set; }
    }

    public class TypeMemberValue
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}
