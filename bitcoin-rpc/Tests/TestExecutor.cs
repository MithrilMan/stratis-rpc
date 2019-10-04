using StratisRpc.CallRequest;
using StratisRpc.Performance;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace StratisRpc.Tests
{
    public static class TestExecutor
    {
        /// <summary>
        /// The maximum retry numbers in case a call fails.
        /// This tries to solve the intermittent problem with "The server returned an invalid or unrecognized response" error
        /// </summary>
        const int MAX_RETRY = 5;

        /// <summary>
        /// The services to test
        /// </summary>
        private static IRpcService[] services;

        public static void SetupServices(params IRpcService[] services)
        {
            TestExecutor.services = services.ToArray(); //creates a copy
        }

        public static void CallNTimes(TestRequest request, int count, PerformanceCollectorOptions options, TestResultCollector testResultCollector = null)
        {
            List<TestResult>[] testResults = Enumerable.Range(0, count).Select((i) => new List<TestResult>()).ToArray();

            foreach (var service in TestExecutor.services)
            {
                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), options))
                {
                    performanceCollector.AddText($"Call {request.MethodToTest} sequentially {count} times\n");

                    for (int i = 0; i < count; i++)
                    {
                        int retries = 0;
                        while (retries < MAX_RETRY)
                        {
                            try
                            {
                                PerformanceEntry performance = performanceCollector.Measure(() => service.CallSingle(request));
                                testResults[i].Add(new TestResult(service, count, performance));
                                break;
                            }
                            catch (System.Exception ex)
                            {
                                Thread.Sleep(200); //introduces a wait between calls because of intermittent problem with "The server returned an invalid or unrecognized response" error
                                retries++;
                            }
                        }
                    }
                }
            }

            if (testResultCollector != null)
            {
                for (int i = 0; i < testResults.Length; i++)
                {
                    testResultCollector.Collect($"t-{i + 1}", testResults[i]);
                }
            }
            else
            {
                using (testResultCollector = new TestResultCollector($"{request.MethodToTest} repeated calls ({count})."))
                {
                    for (int i = 0; i < testResults.Length; i++)
                    {
                        testResultCollector.Collect($"t-{i + 1}", testResults[i]);
                    }
                }
            }
        }

        public static List<TestResult> CallBatch(TestRequest request, int batchSize, PerformanceCollectorOptions options, TestResultCollector testResultCollector = null)
        {
            var testResults = new List<TestResult>();

            foreach (var service in TestExecutor.services)
            {
                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), options))
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
                using (testResultCollector = new TestResultCollector($"{request.MethodToTest} BATCH calls"))
                {
                    testResultCollector.Collect(batchSize.ToString(), testResults);
                }
            }

            return testResults;
        }
    }
}