using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetRawTransaction : TestBase<GetRawTransaction>
    {
        public GetRawTransaction() : base(MethodToTest.GetRawTransaction) { }

        public override GetRawTransaction Batch(string title = null, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0) batchSizes = this.RangeExponential(15, 2, 4).ToArray();

            return base.Batch(title, requestFactory, batchSizes);
        }
    }
}
