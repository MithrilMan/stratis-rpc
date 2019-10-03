using System;
using System.Collections.Generic;
using StratisRpc.OutputFormatter;

namespace StratisRpc.Performance
{
    public class PerformanceCollector : IDisposable
    {
        private readonly bool showCallResult;
        private readonly OutputWriter writer;
        private List<PerformanceEntry> entries;
        private TableBuilder summaryTable;

        public string Context { get; }


        public PerformanceCollector(string context, bool showCallResult, OutputWriter writer = null)
        {
            this.Context = context ?? String.Empty;
            this.showCallResult = showCallResult;
            this.writer = writer ?? new OutputWriter();
            this.entries = new List<PerformanceEntry>();

            this.writer.WriteLine(String.Empty);
            this.writer.DrawLine('=');
            this.writer.WriteLine(context.Center());
            this.writer.DrawLine('=');

            this.summaryTable = new TableBuilder(this.writer)
                    .AddColumn(new ColumnDefinition { Label = "Index", Width = 8, Alignment = ColumnAlignment.Left })
                    .AddColumn(new ColumnDefinition { Label = "Elapsed", Width = 20, Alignment = ColumnAlignment.Right })
                    .Prepare();
        }


        protected void AddResult(PerformanceEntry entry)
        {
            this.entries.Add(entry);

            this.summaryTable.DrawRow($"t-{entries.Count}", entries[entries.Count - 1].Elapsed.TotalMilliseconds.ToString());
        }

        public void AddText(string text)
        {
            this.writer.WriteLine(text);
        }


        /// <summary>
        /// Measures the specified action logging its PerformanceEntry.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public PerformanceEntry Measure(Func<PerformanceEntry> action)
        {
            PerformanceEntry entry = action();
            this.AddResult(entry);

            return entry;
        }

        /// <summary>
        /// Dumps the summary of the performance entries on console.
        /// </summary>
        private void DumpOnConsole()
        {
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.summaryTable.Start();

            for (int index = 0; index < entries.Count; index++)
            {
                this.summaryTable.DrawRow($"t-{index + 1}", entries[index].Elapsed.TotalMilliseconds.ToString());
            }

            if (this.showCallResult)
            {
                this.writer.WriteLine();
                this.writer.DrawLine('*');
                this.writer.WriteLine("RESULTS".Center('*'));
                for (int index = 0; index < entries.Count; index++)
                {
                    PerformanceEntry entry = entries[index];
                    this.writer.WriteLine($"t-{index}: { entry.Result}");
                }
                this.writer.DrawLine('*');
                this.writer.WriteLine();
            }

            this.writer.DrawLine('=');
            this.writer.DrawLine('=');
            this.writer.WriteLine();
        }

        public void Dispose()
        {
            this.DumpOnConsole();
        }
    }
}