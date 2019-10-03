using StratisRpc.CallRequest;
using StratisRpc.Performance;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc.Tests
{
    public static class TestExecutor
    {
        /// <summary>
        /// The services to test
        /// </summary>
        private static IRpcService[] services;

        public static void SetupServices(params IRpcService[] services)
        {
            TestExecutor.services = services.ToArray(); //creates a copy
        }

        public static void CallNTimes(TestRequest request, int count, bool showPartialResult, TestResultCollector testResultCollector = null)
        {
            List<TestResult>[] testResults = Enumerable.Range(0, count).Select((i) => new List<TestResult>()).ToArray();

            foreach (var service in TestExecutor.services)
            {
                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), showPartialResult))
                {
                    performanceCollector.AddText($"Call {request.MethodToTest} sequentially {count} times\n");

                    for (int i = 0; i < count; i++)
                    {
                        PerformanceEntry performance = performanceCollector.Measure(() => service.CallSingle(request));
                        testResults[i].Add(new TestResult(service, count, performance));
                    }
                }
            }

            if (testResultCollector != null)
            {
                for (int i = 0; i < testResults.Length; i++)
                {
                    testResultCollector.Collect($"t-{i}", testResults[i]);
                }
            }
            else
            {
                using (testResultCollector = new TestResultCollector(request.MethodToTest.ToString()))
                {
                    for (int i = 0; i < testResults.Length; i++)
                    {
                        testResultCollector.Collect($"t-{i}", testResults[i]);
                    }
                }
            }
        }

        public static List<TestResult> CallBatch(TestRequest request, int batchSize, bool showPartialResult, TestResultCollector testResultCollector = null)
        {
            var testResults = new List<TestResult>();

            foreach (var service in TestExecutor.services)
            {
                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), showPartialResult))
                {
                    performanceCollector.AddText($"Call Batch RPC of { batchSize} {request.MethodToTest} requests.");
                    PerformanceEntry performance = performanceCollector.Measure(() => service.CallBatch(Enumerable.Range(0, batchSize).Select(n => request).ToList()));
                    testResults.Add(new TestResult(service, batchSize, performance));
                }
            }

            if (testResultCollector != null)
            {
                testResultCollector.Collect(batchSize.ToString(), testResults);
            }
            else
            {
                using (testResultCollector = new TestResultCollector(request.MethodToTest.ToString()))
                {
                    testResultCollector.Collect(batchSize.ToString(), testResults);
                }
            }

            return testResults;
        }
    }
}