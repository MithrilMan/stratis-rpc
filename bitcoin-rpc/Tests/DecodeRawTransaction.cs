using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class DecodeRawTransaction : TestBase<DecodeRawTransaction>
    {
        public DecodeRawTransaction() : base(MethodToTest.DecodeRawTransaction) { }

        public override DecodeRawTransaction Batch(string title = null, bool showResult = false, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, requestFactory, Range(0, 1000, 50).ToArray());
        }
    }
}
