using BitcoinLib.Responses;
using BitcoinLib.RPC.RequestResponse;
using Newtonsoft.Json;
using StratisRpc.Performance;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc.Tests
{
    public class Scenarios : TestBase<Scenarios>
    {
        public Scenarios() : base(MethodToTest.GetBlockCount) { } //here the method to test isn't important, will be ignored by this class

        public Scenarios NodeStatus(bool showStats)
        {
            if (!Enabled)
                return this;

            using (var testResultCollector = new TestResultCollector($"Node Status."))
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

        public Scenarios CheckAllMethods(bool waitBetweenTests)
        {
            if (!Enabled)
                return this;

            new Tests.GetBlockCount()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.GetTransaction()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.GetRawTransaction()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.DecodeRawTransaction()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.ValidateAddress()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.GetBlockHash()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.GetBlock()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.GetBalance()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.ListUnspent()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.ListAddressGroupings()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            new Tests.SendMany()
                .SetOptions(this.options)
                .Execute(5)
                //.Batch()
                .Wait(waitBetweenTests);

            return this;
        }



        public Scenarios TestSendMany(int destinationsCount = 10, decimal amountPerDestination = 1, bool showStats = true)
        {
            if (!Enabled)
                return this;

            CallRequest.SendMany requestFactory(IRpcService service)
            {
                List<SendMany.Destination> destinations = new List<SendMany.Destination>(destinationsCount);
                for (int i = 0; i < destinationsCount; i++)
                {
                    destinations.Add(new SendMany.Destination(TestExecutor.TestData[service].GetAddress(i), amountPerDestination, i % 2 == 0));
                }

                var amounts = destinations.ToDictionary(d => d.DestinationAddress, d => d.Amount);
                string jsonAmounts = JsonConvert.SerializeObject(amounts);

                var subtractFees = destinations.Where(dest => dest.SubtractFees).Select(dest => dest.DestinationAddress).Distinct().ToArray();
                string jsonSubtractFees = null;
                // string jsonSubtractFees = JsonConvert.SerializeObject(subtractFees);

                CallRequest.SendMany request = new CallRequest.SendMany(string.Empty, jsonAmounts, 0, string.Empty, jsonSubtractFees, null, null, null);
                return request;
            }



            using (var testResultCollector = new TestResultCollector($"SendMany: Sending to {destinationsCount} destinations {amountPerDestination} STRAT each."))
            {
                this.CallNTimes(requestFactory, 1, this.options, testResultCollector);

                TestResult testResult = testResultCollector.Results.First().Results.First();

                var parsedResponse = JsonConvert.DeserializeObject<JsonRpcResponse<string>>(testResult.PerformanceEntry.Result);
                string producedTxId = parsedResponse.Result;

                if (parsedResponse.Error != null)
                {
                    testResultCollector.Writer.WriteLine($"Error: {testResult.Service.Name} - {parsedResponse.Error.Message}");
                }
                else
                {
                    if (showStats)
                        testResultCollector.Writer.WriteLine($"Tx-Id: {producedTxId}");
                }

                //new GetTransaction()
                //    .SetOptions(this.options)
                //    .GetSpecificTransaction(producedTxId, 1, true)
                //    ;

            }

            return this;
        }
    }
}
