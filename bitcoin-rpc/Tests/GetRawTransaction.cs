using StratisRpc.CallRequest;

namespace StratisRpc.Tests
{
    public static class GetRawTransaction
    {
        public static void Batch()
        {
            var request = TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction);

            using (var collector = new TestResultCollector("GetRawTransaction Batch"))
            {
                TestExecutor.CallBatch(request, 1, false, collector);
                TestExecutor.CallBatch(request, 10, false, collector);
                TestExecutor.CallBatch(request, 15, false, collector);
                TestExecutor.CallBatch(request, 30, false, collector);
                TestExecutor.CallBatch(request, 60, false, collector);
                TestExecutor.CallBatch(request, 120, false, collector);
            }
        }

        public static void Execute(int count)
        {
            TestExecutor.CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), count, false);
        }
    }
}
