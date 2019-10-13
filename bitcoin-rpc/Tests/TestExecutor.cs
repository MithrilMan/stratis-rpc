using StratisRpc.CallRequest;
using StratisRpc.OutputFormatter;
using StratisRpc.Performance;
using System;
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
        public static IRpcService[] Services { get; private set; }

        public static Dictionary<IRpcService, NodeTestData> TestData { get; private set; }


        public static void SetupServices(OutputWriter writer, params IRpcService[] services)
        {
            TestExecutor.Services = services.ToArray(); //creates a copy

            TestData = new Dictionary<IRpcService, NodeTestData>();

            foreach (var service in services)
            {
                TestData.Add(service, new NodeTestData(service, writer));
            }
        }

        public static void CallNTimes(Func<IRpcService, TestRequest> requestFactory, int count, PerformanceCollectorOptions options, TestResultCollector testResultCollector = null)
        {
            string requestMethodName = null;

            List<TestResult>[] testResults = Enumerable.Range(0, count).Select((i) => new List<TestResult>()).ToArray();

            foreach (var service in TestExecutor.Services)
            {
                var request = requestFactory(service);
                if (requestMethodName == null)
                    requestMethodName = request.MethodToTest.ToString();

                if (options.Enabled)
                    options?.Writer?
                        .WriteLine()
                        .WriteLine($"Calling {requestMethodName} {count} times".Center(80, '~'))
                        .WriteLine(request.ToString())
                        .DrawLine('~');

                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), options))
                {
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
                using (testResultCollector = new TestResultCollector($"{requestMethodName} repeated calls ({count})."))
                {
                    for (int i = 0; i < testResults.Length; i++)
                    {
                        testResultCollector.Collect($"t-{i + 1}", testResults[i]);
                    }
                }
            }
        }

        public static List<TestResult> CallBatch(Func<IRpcService, TestRequest> requestFactory, int batchSize, PerformanceCollectorOptions options, TestResultCollector testResultCollector = null)
        {
            string requestMethodName = null;
            var testResults = new List<TestResult>();

            foreach (var service in TestExecutor.Services)
            {
                var request = requestFactory(service);
                if (requestMethodName == null)
                    requestMethodName = request.MethodToTest.ToString();

                if (options.Enabled)
                    options?.Writer?
                        .WriteLine()
                        .WriteLine($"Calling batch of {batchSize} {requestMethodName}".Center(80, '~'))
                        .WriteLine(request.ToString())
                        .DrawLine('~');

                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), options))
                {
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
                using (testResultCollector = new TestResultCollector($"{requestMethodName} BATCH calls"))
                {
                    testResultCollector.Collect(batchSize.ToString(), testResults);
                }
            }

            return testResults;
        }
    }
}