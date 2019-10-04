using StratisRpc.CallRequest;
using StratisRpc.Performance;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StratisRpc.Tests
{
    public abstract class TestBase<T> where T : TestBase<T>
    {
        /// <summary>
        /// The performance collector options to use during RPC calls.
        /// </summary>
        protected PerformanceCollectorOptions options;

        /// <summary>
        /// If false, prevent any call to RPC
        /// </summary>
        protected bool Enabled { get; private set; }

        public MethodToTest MethodToTest { get; }

        public TestRequest DefaultRequest { get; set; }

        public TestBase(MethodToTest methodToTest)
        {
            this.MethodToTest = methodToTest;

            this.DefaultRequest = TestRequestFactory.CreateRequestFor(methodToTest);
            this.options = PerformanceCollectorOptions.Default;
            this.Enabled = true;
        }

        protected void CallNTimes(TestRequest request, int count, PerformanceCollectorOptions options, TestResultCollector testResultCollector = null)
        {
            TestExecutor.CallNTimes(request, count, options, testResultCollector);
        }

        protected List<TestResult> CallBatch(TestRequest request, int batchSize, PerformanceCollectorOptions options, TestResultCollector testResultCollector = null)
        {
            return TestExecutor.CallBatch(request, batchSize, options, testResultCollector);
        }

        public virtual T Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            if (Enabled)
            {
                if (batchSizes.Length == 0)
                    batchSizes = new int[] { 1, 5, 10 };

                title = title ?? $"{this.DefaultRequest.ToString()} Batch";
                request = request ?? this.DefaultRequest;

                using (var collector = new TestResultCollector(title))
                {
                    for (int i = 0; i < batchSizes.Length; i++)
                        this.CallBatch(request, batchSizes[i], options, collector);
                }
            }

            return (T)this;
        }

        public virtual T Execute(int count = 20, TestRequest request = null)
        {
            if (Enabled)
            {
                this.CallNTimes(request ?? this.DefaultRequest, count, options);
            }

            return (T)this;
        }

        public T SetOptions(PerformanceCollectorOptions options)
        {
            this.options = options;
            return (T)this;
        }


        /// <summary>
        /// Waits a console input before proceeding.
        /// </summary>
        public T Wait()
        {
            if (Enabled)
            {
                Console.WriteLine("Press any key to continue...");
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(0);
                }
                Console.ReadKey(true);
            }

            return (T)this;
        }

        /// <summary>
        /// Useful to prevent this test to run any call
        /// </summary>
        public T Disable()
        {
            this.Enabled = false;

            return (T)this;
        }


        protected IEnumerable<int> Range(int from = 0, int to = 100, int step = 20, bool includeOne = true)
        {
            if (includeOne)
                yield return 1;

            for (int i = from; i <= to; i += step)
            {
                if (i > 1)
                    yield return i;
            }
        }

        protected IEnumerable<int> RangeExponential(int from = 1, int multiplier = 2, int items = 5, bool includeOne = true)
        {
            if (includeOne)
                yield return 1;

            int returnedValue = from;
            if (returnedValue > 1)
            {
                items--;
                yield return from;
            }

            for (int i = 1; i < items; i++)
            {
                returnedValue = returnedValue * multiplier;
                yield return returnedValue;
            }
        }
    }
}