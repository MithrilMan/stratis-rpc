using StratisRpc.CallRequest;

namespace StratisRpc.Tests
{
    public class GetBlockCount : TestBase<GetBlockCount>
    {
        public GetBlockCount() : base(MethodToTest.GetBlockCount) { }

        public override GetBlockCount Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, 120, 240, 480, 960);
        }
    }
}
