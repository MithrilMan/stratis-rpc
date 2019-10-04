﻿using BitcoinLib.Responses;
using BitcoinLib.RPC.RequestResponse;
using Newtonsoft.Json;
using StratisRpc.CallRequest;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc.Tests
{
    public class Scenarios : TestBase<Scenarios>
    {
        public Scenarios() : base(MethodToTest.GetTransaction) { } //here the method to test isn't important, will be ignored by this class

        public Scenarios NodeStatus(bool showStats)
        {
            using (var testResultCollector = new TestResultCollector($"Node Status."))
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

        public Scenarios CheckAllMethods()
        {
            if (!Enabled)
                return this;

            new Tests.GetBlockCount()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.GetTransaction()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.GetRawTransaction()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.DecodeRawTransaction()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.DecodeRawTransaction()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.ValidateAddress()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.GetBlockHash()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.GetBlock()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.GetBalance()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.ListUnspent()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            new Tests.ListAddressGroupings()
                .SetOptions(this.options)
                .Execute(20)
                .Batch();

            /// SendMany **W

            return this;
        }
    }
}