using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class DecodeRawTransaction : TestBase<DecodeRawTransaction>
    {
        public DecodeRawTransaction() : base(MethodToTest.DecodeRawTransaction) { }

        public override DecodeRawTransaction Batch(string title = null, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0) batchSizes = this.Range(0, 1000, 50).ToArray();

            return base.Batch(title, requestFactory, batchSizes);
        }
    }
}
