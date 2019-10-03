using StratisRpc.CallRequest;
using System.Collections.Generic;

namespace StratisRpc.Tests
{
    public abstract class TestBase<T> where T : TestBase<T>
    {
        public MethodToTest MethodToTest { get; }

        public TestRequest DefaultRequest { get; set; }

        public TestBase(MethodToTest methodToTest)
        {
            this.MethodToTest = methodToTest;

            this.DefaultRequest = TestRequestFactory.CreateRequestFor(methodToTest);
        }

        protected void CallNTimes(TestRequest request, int count, bool showPartialResult, TestResultCollector testResultCollector = null)
        {
            TestExecutor.CallNTimes(request, count, showPartialResult, testResultCollector);
        }

        protected List<TestResult> CallBatch(TestRequest request, int batchSize, bool showPartialResult, TestResultCollector testResultCollector = null)
        {
            return TestExecutor.CallBatch(request, batchSize, showPartialResult, testResultCollector);
        }

        public virtual T Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0)
                batchSizes = new int[] { 1, 5, 10 };

            title = title ?? $"{this.DefaultRequest.ToString()} Batch";
            request = request ?? this.DefaultRequest;

            using (var collector = new TestResultCollector(title))
            {
                for (int i = 0; i < batchSizes.Length; i++)
                    this.CallBatch(request, batchSizes[i], showResult, collector);
            }

            return (T)this;
        }

        public virtual T Execute(int count = 20, TestRequest request = null)
        {
            this.CallNTimes(request ?? this.DefaultRequest, count, false);
            return (T)this;
        }
    }
}