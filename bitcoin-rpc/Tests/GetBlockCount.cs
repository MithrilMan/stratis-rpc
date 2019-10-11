using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlockCount : TestBase<GetBlockCount>
    {
        public GetBlockCount() : base(MethodToTest.GetBlockCount) { }

        public override GetBlockCount Batch(string title = null, bool showResult = false, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, requestFactory, this.RangeExponential(120, 2, 4).ToArray());
        }
    }
}
