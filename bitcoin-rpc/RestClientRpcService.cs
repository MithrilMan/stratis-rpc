using BitcoinLib.RPC.RequestResponse;
using Easy.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bitcoin_rpc
{
    public class RestClientRpcService : RpcService
    {
        private string rpcUrl;
        protected readonly RestClient restClient;

        public RestClientRpcService(string friendlyName, IPEndPoint rpcEndpoint, string rpcUser, string rpcPassword, string walletPassword, short timeoutInSeconds = 60)
            : base(friendlyName, rpcEndpoint, rpcUser, rpcPassword, walletPassword, timeoutInSeconds)
        {
            this.rpcUrl = $"http://{rpcEndpoint.Address}:{rpcEndpoint.Port}";

            this.restClient = CreateRestClient();
        }

        protected virtual RestClient CreateRestClient()
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

            return new RestClient(defaultHeaders, handler, timeout: TimeSpan.FromSeconds(20));
        }

        protected override string CallSingleImpl()
        {
            var requestPayload = new JsonRpcRequest(1, "getrawtransaction", TestData.GetTxId(0), 0);

            var result = this.RpcCall(requestPayload.GetBytes()).Result; ;
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
