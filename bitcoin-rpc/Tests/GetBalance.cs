using StratisRpc.CallRequest;

namespace StratisRpc.Tests
{
    public static class GetBalance
    {
        public static void Execute(int count)
        {
            using (var collector = new TestResultCollector("GetBalance"))
            {
                TestExecutor.CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetBalance), count, true, collector);
            }
        }
    }
}
