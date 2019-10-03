using StratisRpc.CallRequest;

namespace StratisRpc.Tests
{
    public static class GetTransaction
    {
        public static void Batch()
        {
            var request = TestRequestFactory.CreateRequestFor(MethodToTest.GetTransaction);
            using (var collector = new TestResultCollector("GetTransaction Batch"))
            {
                TestExecutor.CallBatch(request, 1, true, collector);
                TestExecutor.CallBatch(request, 5, true, collector);
                TestExecutor.CallBatch(request, 10, true, collector);
            }
        }

        public static void Execute(int count)
        {
            TestExecutor.CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetTransaction), count, false);
        }
    }
}
