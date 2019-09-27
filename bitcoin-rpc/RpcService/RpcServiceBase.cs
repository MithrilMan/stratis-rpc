using BitcoinLib.RPC.RequestResponse;
using Newtonsoft.Json;
using StratisRpc.CallRequest;
using StratisRpc.Performance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace StratisRpc.RpcService
{
    public abstract class RpcServiceBase : IRpcService
    {
        protected readonly string friendlyName;
        private readonly IPEndPoint rpcEndpoint;
        protected readonly string rpcUser;
        protected readonly string rpcPassword;
        protected readonly string walletPassword;
        protected readonly short timeoutInSeconds;

        public string Name => friendlyName;

        public RpcServiceBase(string friendlyName, IPEndPoint rpcEndpoint, string rpcUser, string rpcPassword, string walletPassword, short timeoutInSeconds = 60)
        {
            this.friendlyName = friendlyName;
            this.rpcEndpoint = rpcEndpoint;
            this.rpcUser = rpcUser;
            this.rpcPassword = rpcPassword;
            this.walletPassword = walletPassword;
            this.timeoutInSeconds = timeoutInSeconds;
        }

        protected abstract string CallSingleImpl(TestRequest request);
        protected abstract string CallBatchImpl(byte[] batchPayload);

        /// <inheritdoc />
        public PerformanceEntry CallSingle(TestRequest request)
        {
            TimeSpan elapsedTime = TimeSpan.Zero;
            string result;

            using (new StopwatchDisposable(ticks => elapsedTime = ticks))
            {
                result = this.CallSingleImpl(request);
            }

            return new PerformanceEntry(elapsedTime, result);
        }

        /// <inheritdoc />
        public PerformanceEntry CallBatch(List<TestRequest> requests)
        {
            List<string> batchedRequests = requests
                .Select((request, index) => request.ToJson(index))
                .ToList();

            var byteArray = Encoding.UTF8.GetBytes($"[{string.Join(',', batchedRequests)}]");

            string result;
            TimeSpan elapsedTime = TimeSpan.Zero;
            using (new StopwatchDisposable(ticks => elapsedTime = ticks))
            {
                result = CallBatchImpl(byteArray);
            }

            return new PerformanceEntry(elapsedTime, result);
        }

        public string GetServiceDescription()
        {
            return $"{this.friendlyName}({this.GetType().Name}): url =  {rpcEndpoint.Address}:{rpcEndpoint.Port}";
        }
    }
}
