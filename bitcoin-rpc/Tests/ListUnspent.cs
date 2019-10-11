using BitcoinLib.Responses;
using BitcoinLib.RPC.RequestResponse;
using Newtonsoft.Json;
using StratisRpc.CallRequest;
using StratisRpc.Performance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc.Tests
{
    public class ListUnspent : TestBase<ListUnspent>
    {
        public ListUnspent() : base(MethodToTest.ListUnspent) { }

        public override ListUnspent Batch(string title = null, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0) batchSizes = this.Range(0, 5, 1).ToArray();

            return base.Batch(title, requestFactory, batchSizes);
        }

        public ListUnspent Single(bool showStats = true)
        {
            if (!Enabled)
                return this;

            using (var testResultCollector = new TestResultCollector($"Single ListUnspent call."))
            {
                CallRequest.ListUnspent request = new CallRequest.ListUnspent(null, null, null, null, null);
                this.CallNTimes(_ => request, 1, this.options, testResultCollector);

                TestResult testResult = testResultCollector.Results.First().Results.First();

                var parsedResponse = JsonConvert.DeserializeObject<JsonRpcResponse<List<ListUnspentResponse>>>(testResult.PerformanceEntry.Result);

                if (showStats)
                    testResultCollector.Writer.WriteLine($"UnspentCoins: {parsedResponse.Result.Count}");
            }

            return this;
        }
    }
}
