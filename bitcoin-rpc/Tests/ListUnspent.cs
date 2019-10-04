using BitcoinLib.Responses;
using BitcoinLib.RPC.RequestResponse;
using Newtonsoft.Json;
using StratisRpc.CallRequest;
using StratisRpc.Performance;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc.Tests
{
    public class ListUnspent : TestBase<ListUnspent>
    {
        public ListUnspent() : base(MethodToTest.ListUnspent) { }

        public override ListUnspent Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, 1, 2, 3, 4, 5);
        }

        public ListUnspent Single(bool showStats = true)
        {
            using (var testResultCollector = new TestResultCollector($"Single ListUnspent call."))
            {
                CallRequest.ListUnspent request = new CallRequest.ListUnspent(null, null, null, null, null);
                this.CallNTimes(request, 1, this.options, testResultCollector);

                TestResult testResult = testResultCollector.Results.First().Results.First();

                var parsedResponse = JsonConvert.DeserializeObject<JsonRpcResponse<List<ListUnspentResponse>>>(testResult.PerformanceEntry.Result);

                if (showStats)
                    testResultCollector.Writer.WriteLine($"UnspentCoins: {parsedResponse.Result.Count}");
            }

            return this;
        }
    }
}
