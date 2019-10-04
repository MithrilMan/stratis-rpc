using StratisRpc.CallRequest;
using System.Linq;

namespace StratisRpc.Tests
{
    public class GetBlock: TestBase<GetBlock>
    {
        public GetBlock() : base(MethodToTest.GetBlock) { }

        public override GetBlock Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, this.Range(0, 200, 50).ToArray());
        }
    }
}
