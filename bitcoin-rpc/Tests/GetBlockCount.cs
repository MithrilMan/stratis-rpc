using StratisRpc.CallRequest;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlockCount : TestBase<GetBlockCount>
    {
        public GetBlockCount() : base(MethodToTest.GetBlockCount) { }

        public override GetBlockCount Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, this.RangeExponential(120, 2, 4).ToArray());
        }
    }
}
