using BitcoinLib.RPC.RequestResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace StratisRpc
{
    public abstract class RpcService : IRpcService
    {
        protected readonly string friendlyName;
        private readonly IPEndPoint rpcEndpoint;
        protected readonly string rpcUser;
        protected readonly string rpcPassword;
        protected readonly string walletPassword;
        protected readonly short timeoutInSeconds;
        private Stopwatch stopwatch;

        public RpcService(string friendlyName, IPEndPoint rpcEndpoint, string rpcUser, string rpcPassword, string walletPassword, short timeoutInSeconds = 60)
        {
            this.friendlyName = friendlyName;
            this.rpcEndpoint = rpcEndpoint;
            this.rpcUser = rpcUser;
            this.rpcPassword = rpcPassword;
            this.walletPassword = walletPassword;
            this.timeoutInSeconds = timeoutInSeconds;

            this.stopwatch = new Stopwatch();
        }

        protected abstract string CallSingleImpl();
        protected abstract string CallBatchImpl(byte[] batchPayload);

        public void CallSingle(bool showResult)
        {
            stopwatch.Restart();
            string result = this.CallSingleImpl();
            stopwatch.Stop();

            if (showResult)
                Console.WriteLine($"CallSingle [OK]: {result}");

            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds} ms");
        }

        public void CallBatch(bool showResult, int batchSize, string message = null)
        {
            if (batchSize < 1)
                return;

            List<JsonRpcRequest> batchedRequests = Enumerable.Range(0, batchSize)
                .Select(n => new JsonRpcRequest(n + 1, "getrawtransaction", TestData.GetTxId(), 0))
                .ToList();

            var byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(batchedRequests));

            Console.WriteLine(message);

            stopwatch.Restart();
            string response = CallBatchImpl(byteArray);
            stopwatch.Stop();

            if (showResult)
                Console.WriteLine($"[OK]: {response}");

            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds} ms");
        }

        public string GetServiceDescription()
        {
            return $"{this.friendlyName}({this.GetType().Name}): url =  {rpcEndpoint.Address}:{rpcEndpoint.Port}";
        }
    }
}
