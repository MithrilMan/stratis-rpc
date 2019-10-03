using StratisRpc.CallRequest;

namespace StratisRpc.Tests
{
    public class GetRawTransaction : TestBase<GetRawTransaction>
    {
        public GetRawTransaction() : base(MethodToTest.GetRawTransaction) { }

        public override GetRawTransaction Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, 1, 10, 15, 30, 60, 120);
        }
    }
}
