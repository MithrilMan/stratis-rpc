using StratisRpc.CallRequest;

namespace StratisRpc.Tests
{
    public class GetTransaction : TestBase<GetTransaction>
    {
        public GetTransaction() : base(MethodToTest.GetTransaction) { }

        public override GetTransaction Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, 1, 5, 10);
        }
    }
}
