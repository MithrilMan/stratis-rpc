using BitcoinLib.RPC.Specifications;
using Newtonsoft.Json;

namespace StratisRpc.CallResponse
{
    public class RpcError
    {
        [JsonProperty(PropertyName = "code")]
        public RpcErrorCode Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
