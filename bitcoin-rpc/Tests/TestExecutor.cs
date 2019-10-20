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

        private static bool initialized = false;

        /// <summary>
        /// The services to test
        /// </summary>
        public static IRpcService[] Services { get; private set; }

        public static Dictionary<IRpcService, NodeTestData> TestData { get; private set; }

        public static Dictionary<IRpcService, TestSummary> SingleCallResults { get; private set; }
        public static Dictionary<IRpcService, TestSummary> BatchCallResults { get; private set; }


        public static void SetupServices(OutputWriter writer, params IRpcService[] services)
        {
            TestExecutor.Services = services.ToArray(); //creates a copy

            TestData = new Dictionary<IRpcService, NodeTestData>();
            SingleCallResults = new Dictionary<IRpcService, TestSummary>();
            BatchCallResults = new Dictionary<IRpcService, TestSummary>();

            foreach (var service in services)
            {
                TestData.Add(service, new NodeTestData(service, writer));
                SingleCallResults.Add(service, new TestSummary(TestData[service]));
                BatchCallResults.Add(service, new TestSummary(TestData[service]));
            }

            initialized = true;
        }

        public static void CallNTimes(Func<IRpcService, TestRequest> requestFactory, int count, PerformanceCollectorOptions options, TestResultCollector testResultCollector = null)
        {
            string requestMethodName = null;

            List<TestResult>[] testResults = Enumerable.Range(0, count).Select((i) => new List<TestResult>()).ToArray();

            foreach (var service in TestExecutor.Services)
            {
                var request = requestFactory(service);
                if (requestMethodName == null)
                    requestMethodName = request.MethodName;

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
                                AddSingleCallResult(service, requestMethodName, performance.Elapsed);
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
                    requestMethodName = request.MethodName;

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
                    AddBatchCallResult(service, requestMethodName, batchSize, performance.Elapsed);
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


        private static void AddSingleCallResult(IRpcService service, string calledMethod, TimeSpan elapsed)
        {
            if (!initialized)
                return;

            if (SingleCallResults.TryGetValue(service, out TestSummary summary))
            {
                summary.RegisterSingleCallResult(calledMethod, elapsed);
            }
        }

        private static void AddBatchCallResult(IRpcService service, string calledMethod, int batchSize, TimeSpan elapsed)
        {
            if (!initialized)
                return;

            if (BatchCallResults.TryGetValue(service, out TestSummary summary))
            {
                summary.RegisterBatchCallResult(calledMethod, batchSize, elapsed);
            }
        }


        public static void DumpSummary(OutputWriter writer, UnitOfTimeFormatter timeFormatter = null)
        {
            DumpNodeSetups(writer);

            DumpSingleCallsSummary(writer, timeFormatter);

            DumpBatchCallsSummary(writer, timeFormatter);
        }

        private static void DumpNodeSetups(OutputWriter writer)
        {
            TableBuilder nodeSetupTable = new TableBuilder(writer)
                .AddColumn(new ColumnDefinition { Label = "Node", Width = 20, Alignment = ColumnAlignment.Left })
                .AddColumn(new ColumnDefinition { Label = "UTXO count", Width = 15, Alignment = ColumnAlignment.Center })
                .AddColumn(new ColumnDefinition { Label = "Address count", Width = 15, Alignment = ColumnAlignment.Center })
                .Prepare()
                .Start();

            foreach (var serviceResult in SingleCallResults)
            {
                nodeSetupTable.DrawRow(serviceResult.Key.Name, serviceResult.Value.UtxoCount.ToString(), serviceResult.Value.AddressCount.ToString());
            }

            nodeSetupTable.End();
        }

        private static void DumpSingleCallsSummary(OutputWriter writer, UnitOfTimeFormatter timeFormatter)
        {
            if (SingleCallResults.Count == 0 || SingleCallResults.First().Value.Details.Count == 0)
                return;

            List<IRpcService> services = SingleCallResults.Keys.ToList();
            List<string> testedMethods = SingleCallResults.First().Value.Details.Keys.ToList();

            writer?.WriteLine("Summary of batch method calls:");
            TableBuilder singleCallsTable = new TableBuilder(writer)
                .AddColumn(new ColumnDefinition { Label = "Tested Method", Width = 1 + testedMethods.Max(methodName => methodName.Length), Alignment = ColumnAlignment.Left })
                .AddColumn(new ColumnDefinition { Label = "Samples", Width = 10, Alignment = ColumnAlignment.Left })
                .AddColumns(services.Select(service => new ColumnDefinition { Label = $"{service.Name} (min; max; avg)", Width = 30, Alignment = ColumnAlignment.Left }).ToArray())
                .Prepare()
                .Start()
                ;

            foreach (string testedMethod in testedMethods)
            {
                List<string> values = new List<string> {
                    testedMethod, //tested method
                    SingleCallResults.Values.FirstOrDefault()?.Details[testedMethod].Count.ToString() // samples count.
                };

                values.AddRange(
                    from service in services
                    let serviceResults = SingleCallResults[service].Details[testedMethod]
                    let min = timeFormatter.Format(serviceResults.Min(r => r.Elapsed), false)
                    let max = timeFormatter.Format(serviceResults.Max(r => r.Elapsed), false)
                    let average = timeFormatter.Format(TimeSpan.FromTicks((long)serviceResults.Average(r => r.Elapsed.Ticks)), false)
                    select $"{min}; {max}; {average} ({timeFormatter.GetUnitString()})"
                );

                singleCallsTable.DrawRow(values.ToArray());
            }

            singleCallsTable.End();
        }

        private static void DumpBatchCallsSummary(OutputWriter writer, UnitOfTimeFormatter timeFormatter)
        {
            if (BatchCallResults.Count == 0 || BatchCallResults.First().Value.Details.Count == 0)
                return;

            List<IRpcService> services = BatchCallResults.Keys.ToList();
            List<string> testedMethods = BatchCallResults.First().Value.Details.Keys.ToList();

            writer?.WriteLine("Summary of single method calls:");
            TableBuilder table = new TableBuilder(writer)
                .AddColumn(new ColumnDefinition { Label = "Tested Method", Width = 1 + testedMethods.Max(methodName => methodName.Length), Alignment = ColumnAlignment.Left })
                .AddColumns(services.Select(service => new ColumnDefinition { Label = $"{service.Name} (total; per-item)", Width = 30, Alignment = ColumnAlignment.Left }).ToArray())
                .Prepare()
                .Start()
                ;

            foreach (string testedMethod in testedMethods)
            {
                List<string> values = new List<string>();
                values.Add(testedMethod);
                values.AddRange(
                    from service in services
                    let serviceResults = BatchCallResults[service].Details[testedMethod]
                    let result = serviceResults.FirstOrDefault() as TestSummary.BatchDetail
                    let total = timeFormatter.Format(result.Elapsed)
                    let perItem = timeFormatter.Format(result.Elapsed / result.BatchSize)
                    select $"{total}; {perItem} ({timeFormatter.GetUnitString()})"
                );

                table.DrawRow(values.ToArray());
            }

            table.End();
        }
    }
}