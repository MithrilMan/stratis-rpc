using StratisRpc.OutputFormatter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc
{
    public class TestResultCollector : IDisposable
    {
        private readonly string context;
        private readonly OutputWriter writer;
        private readonly List<(string TestCase, List<TestResult> Results)> results;

        public TestResultCollector(string context, OutputWriter writer = null)
        {
            this.context = context;
            this.writer = writer ?? new OutputWriter();

            this.results = new List<(string TestCase, List<TestResult> Results)>();
        }

        public void Collect(string testCase, List<TestResult> results)
        {
            this.results.Add((testCase, results));
        }

        public void Dispose()
        {
            this.writer.WriteLine();
            this.writer.DrawLine('^');
            this.writer.WriteLine($"TEST RESULTS: {context}");

            if (results.Count > 0)
            {
                var firstResult = results.First().Results;
                TableBuilder table = new TableBuilder(writer)
                    .AddColumn(new ColumnDefinition { Label = "Test Case", Width = 10, Alignment = ColumnAlignment.Left })
                    .AddColumns(firstResult.Select(item => new ColumnDefinition
                    {
                        Alignment = ColumnAlignment.Left,
                        Label = item.Service.Name,
                        Width = 15
                    }).ToArray())
                    .Start();

                foreach (var result in results)
                {
                    List<string> values = new List<string> { result.TestCase };
                    values.AddRange(result.Results.Select(r => r.PerformanceEntry.Elapsed.TotalMilliseconds.ToString()));
                    table.DrawRow(values.ToArray());
                }
            }

            this.writer.DrawLine('^');
        }
    }
}
