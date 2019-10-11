using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlockHash : TestBase<GetBlockHash>
    {
        public GetBlockHash() : base(MethodToTest.GetBlockHash) { }

        public override GetBlockHash Batch(string title = null, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0) batchSizes = this.Range(0, 1000, 50).ToArray();

            return base.Batch(title, requestFactory, batchSizes);
        }
    }
}
