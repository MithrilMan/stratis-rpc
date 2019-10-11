using Newtonsoft.Json;

namespace StratisRpc.CallResponse
{
    public class RpcResponse<T>
    {
        [JsonProperty(PropertyName = "result", Order = 0)]
        public T Result { get; set; }

        [JsonProperty(PropertyName = "id", Order = 1)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "error", Order = 2)]
        public RpcError Error { get; set; }
    }
}
