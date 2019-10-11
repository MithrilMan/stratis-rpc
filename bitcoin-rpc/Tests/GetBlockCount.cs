using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlockCount : TestBase<GetBlockCount>
    {
        public GetBlockCount() : base(MethodToTest.GetBlockCount) { }

        public override GetBlockCount Batch(string title = null, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0) batchSizes = this.RangeExponential(120, 2, 4).ToArray();

            return base.Batch(title, requestFactory, batchSizes);
        }
    }
}
