using StratisRpc.OutputFormatter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc
{
    public class TestResultCollector : IDisposable
    {
        private readonly string context;

        public OutputWriter Writer { get; }

        public List<(string TestCase, List<TestResult> Results)> Results { get; }

        public TestResultCollector(string context, OutputWriter writer = null)
        {
            this.context = context;
            this.Writer = writer ?? new OutputWriter();

            this.Results = new List<(string TestCase, List<TestResult> Results)>();
        }

        public void Collect(string testCase, List<TestResult> results)
        {
            this.Results.Add((testCase, results));
        }

        public void Dispose()
        {
            this.Writer.WriteLine();
            this.Writer.DrawLine('^');
            this.Writer.WriteLine($"TEST RESULTS: {context}");

            if (Results.Count > 0)
            {
                var firstResult = Results.First().Results;
                TableBuilder table = new TableBuilder(Writer)
                    .AddColumn(new ColumnDefinition { Label = "Test Case", Width = 10, Alignment = ColumnAlignment.Left })
                    .AddColumns(firstResult.Select(item => new ColumnDefinition
                    {
                        Alignment = ColumnAlignment.Left,
                        Label = item.Service.Name,
                        Width = 15
                    }).ToArray())
                    .Start();

                foreach (var result in Results)
                {
                    List<string> values = new List<string> { result.TestCase };
                    values.AddRange(result.Results.Select(r => r.PerformanceEntry.Elapsed.TotalMilliseconds.ToString()));
                    table.DrawRow(values.ToArray());
                }
            }

            this.Writer.DrawLine('^');
        }
    }
}
