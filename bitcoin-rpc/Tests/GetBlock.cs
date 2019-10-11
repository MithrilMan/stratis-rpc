using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlock: TestBase<GetBlock>
    {
        public GetBlock() : base(MethodToTest.GetBlock) { }

        public override GetBlock Batch(string title = null, bool showResult = false, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, requestFactory, this.Range(0, 200, 50).ToArray());
        }
    }
}
