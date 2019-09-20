using System;
using System.Diagnostics;
using System.Net;

namespace bitcoin_rpc
{
    public class BitcoinlibRpcService : RpcService
    {
        private string rpcUrl;
        protected StratisService service;

        public BitcoinlibRpcService(string friendlyName, IPEndPoint rpcEndpoint, string rpcUser, string rpcPassword, string walletPassword, short timeoutInSeconds = 60)
            : base(friendlyName, rpcEndpoint, rpcUser, rpcPassword, walletPassword, timeoutInSeconds)
        {
            this.rpcUrl = $"http://{rpcEndpoint.Address}:{rpcEndpoint.Port}";

            this.service = new StratisService(rpcUrl, this.rpcUser, rpcPassword, walletPassword, timeoutInSeconds);
        }

        protected override string CallSingleImpl()
        {
            var tx = service.GetRawTransaction(TestData.GetTxId(), 0);
            return tx?.Hex;
        }

        protected override string CallBatchImpl(byte[] batchPayload)
        {
            string response = this.service.MakeRawBatchRequests(batchPayload, TimeSpan.FromSeconds(this.timeoutInSeconds));
            return response;
        }
    }
}
