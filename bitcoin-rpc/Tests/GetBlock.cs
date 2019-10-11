using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlock : TestBase<GetBlock>
    {
        public GetBlock() : base(MethodToTest.GetBlock) { }

        public override GetBlock Batch(string title = null, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0) batchSizes = this.Range(0, 200, 50).ToArray();

            return base.Batch(title, requestFactory, batchSizes);
        }
    }
}
