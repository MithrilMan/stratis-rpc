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
    public class GetTransaction : TestBase<GetTransaction>
    {
        public GetTransaction() : base(MethodToTest.GetTransaction) { }

        public override GetTransaction Batch(string title = null, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0) batchSizes = this.RangeExponential(5, 2, 2).ToArray();

            return base.Batch(title, requestFactory, batchSizes);
        }



        public GetTransaction GetSpecificTransaction(string txId, int repeatCount = 1, bool showStats = true)
        {
            if (!Enabled)
                return this;

            using (var testResultCollector = new TestResultCollector($"GetTransaction: Get specific tranasction {txId} {repeatCount} times."))
            {
                CallRequest.GetTransaction request = new CallRequest.GetTransaction(txId, null);
                this.CallNTimes(_ => request, repeatCount, this.options, testResultCollector);

                TestResult testResult = testResultCollector.Results.First().Results.First();

                var parsedResponse = JsonConvert.DeserializeObject<JsonRpcResponse<GetTransactionResponse>>(testResult.PerformanceEntry.Result);

                if (parsedResponse.Error != null)
                {
                    testResultCollector.Writer.WriteLine($"Error: {testResult.Service.Name} - {parsedResponse.Error.Message}");
                }
                else
                {
                    if (showStats)
                        testResultCollector.Writer.WriteLine($"Tx-Id: {parsedResponse.Result.TxId}");
                }
            }

            return this;
        }
    }
}
