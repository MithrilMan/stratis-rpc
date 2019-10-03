using StratisRpc.CallRequest;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlockHash : TestBase<GetBlockHash>
    {
        public GetBlockHash() : base(MethodToTest.GetBlockHash) { }

        public override GetBlockHash Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, this.Range(0, 1000, 50).ToArray());
        }
    }
}
