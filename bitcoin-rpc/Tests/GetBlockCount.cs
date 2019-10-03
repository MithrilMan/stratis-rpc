using StratisRpc.CallRequest;

namespace StratisRpc.Tests
{
    public static class GetBlockCount
    {
        public static void Execute(int count)
        {
            TestExecutor.CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), count, false);
        }

        public static void Batch()
        {
            var request = TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount);

            using (var collector = new TestResultCollector("GetBlockCount Batch"))
            {
                TestExecutor.CallBatch(request, 120, false, collector);
                TestExecutor.CallBatch(request, 240, false, collector);
                TestExecutor.CallBatch(request, 480, false, collector);
                TestExecutor.CallBatch(request, 960, false, collector);
            }
        }
    }
}
