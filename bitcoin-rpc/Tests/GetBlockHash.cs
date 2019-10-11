using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlockHash : TestBase<GetBlockHash>
    {
        public GetBlockHash() : base(MethodToTest.GetBlockHash) { }

        public override GetBlockHash Batch(string title = null, bool showResult = false, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, requestFactory, this.Range(0, 1000, 50).ToArray());
        }
    }
}
