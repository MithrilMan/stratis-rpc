using StratisRpc.CallRequest;
using System;
using System.Net;

namespace StratisRpc.RpcService.Bitcoinlib
{
    public class BitcoinlibRpcService : RpcServiceBase
    {
        private string rpcUrl;
        protected StratisService service;

        public BitcoinlibRpcService(string friendlyName, IPEndPoint rpcEndpoint, string rpcUser, string rpcPassword, string walletPassword, short timeoutInSeconds = 60)
            : base(friendlyName, rpcEndpoint, rpcUser, rpcPassword, walletPassword, timeoutInSeconds)
        {
            this.rpcUrl = $"http://{rpcEndpoint.Address}:{rpcEndpoint.Port}";

            this.service = new StratisService(rpcUrl, this.rpcUser, rpcPassword, walletPassword, timeoutInSeconds);
        }

        protected override string CallSingleImpl(TestRequest request)
        {

            switch (request)
            {
                case GetBlockCount req:
                    return service.GetBlockCount().ToString();
                case GetTransaction req:
                    return service.GetTransaction(req.TxId, (bool?)req.IncludeWatchOnly).ToString();
                case GetRawTransaction req:
                    var tx = service.GetRawTransaction(req.TxId, req.Verbose.GetValueOrDefault(0));
                    return tx?.Hex;
                case DecodeRawTransaction req:
                    return service.DecodeRawTransaction(req.HexString).ToString();
                default:
                    throw new NotImplementedException(request.MethodToTest.ToString());

            }

            switch (request.MethodToTest)
            {
                case MethodToTest.ValidateAddress:
                case MethodToTest.GetBlockHash:
                case MethodToTest.GetBlock:
                case MethodToTest.GetBalance:
                case MethodToTest.ListUnspent:
                case MethodToTest.ListAddressGroupings:
                case MethodToTest.SendMany:
                default:
                    throw new NotImplementedException(request.MethodToTest.ToString());
            }
        }

        protected override string CallBatchImpl(byte[] batchPayload)
        {
            string response = this.service.MakeRawBatchRequests(batchPayload, TimeSpan.FromSeconds(this.timeoutInSeconds));
            return response;
        }
    }
}
