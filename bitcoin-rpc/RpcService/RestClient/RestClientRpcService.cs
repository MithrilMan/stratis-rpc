using StratisRpc.CallRequest;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace StratisRpc.RpcService.RestClient
{
    public class RestClientRpcService : RpcServiceBase
    {
        private string rpcUrl;
        protected readonly Easy.Common.RestClient restClient;

        public RestClientRpcService(string friendlyName, IPEndPoint rpcEndpoint, string rpcUser, string rpcPassword, string walletPassword, short timeoutInSeconds = 60)
            : base(friendlyName, rpcEndpoint, rpcUser, rpcPassword, walletPassword, timeoutInSeconds)
        {
            this.rpcUrl = $"http://{rpcEndpoint.Address}:{rpcEndpoint.Port}";

            this.restClient = CreateRestClient();
        }

        protected virtual Easy.Common.RestClient CreateRestClient()
        {
            var defaultHeaders = new Dictionary<string, IEnumerable<string>>{
                { "Accept", new string[] {"application/json-rpc"} },
                {"User-Agent",new string[] { "RPC-Tester" } }
            };

            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(this.rpcUser, this.rpcPassword)
            };

            return new Easy.Common.RestClient(defaultHeaders, handler, timeout: TimeSpan.FromSeconds(20));
        }

        protected override string CallSingleImpl(TestRequest request)
        {
            var result = this.RpcCall(request.GetBytes(0)).Result;
            return result;
        }

        protected override string CallBatchImpl(byte[] batchPayload)
        {
            string response = this.RpcCall(batchPayload).Result;
            return response;
        }


        protected virtual async Task<string> RpcCall(byte[] content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, this.rpcUrl);
            request.Content = new ByteArrayContent(content);
            var response = await restClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
