using System;
using System.Collections.Generic;
using StratisRpc.OutputFormatter;

namespace StratisRpc.Performance
{
    public class PerformanceCollector : IDisposable
    {
        private readonly OutputWriter writer;
        private readonly PerformanceCollectorOptions options;
        private List<PerformanceEntry> entries;
        private TableBuilder summaryTable;

        public string Context { get; }


        public PerformanceCollector(string context, PerformanceCollectorOptions options)
        {
            this.Context = context ?? String.Empty;
            this.options = options ?? new PerformanceCollectorOptions { Enabled = true, ShowResults = false };
            this.writer = options.Writer ?? new OutputWriter();
            this.entries = new List<PerformanceEntry>();

            this.summaryTable = new TableBuilder(this.writer)
                .AddColumn(new ColumnDefinition { Label = "Index", Width = 8, Alignment = ColumnAlignment.Left })
                .AddColumn(new ColumnDefinition { Label = "Elapsed", Width = 20, Alignment = ColumnAlignment.Right })
                .Prepare();

            if (options.Enabled)
            {
                this.writer.WriteLine(String.Empty);
                this.writer.DrawLine('=');
                this.writer.WriteLine(context.Center());
                this.writer.DrawLine('=');
            }
        }


        protected void AddResult(PerformanceEntry entry)
        {
            this.entries.Add(entry);

            if (options.Enabled)
                this.summaryTable.DrawRow($"t-{entries.Count}", entries[entries.Count - 1].Elapsed.TotalMilliseconds.ToString());
        }

        public void AddText(string text)
        {
            if (options.Enabled)
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
            if (!options.Enabled)
                return;

            this.writer.WriteLine();
            this.writer.WriteLine();
            this.summaryTable.Start();

            for (int index = 0; index < entries.Count; index++)
            {
                this.summaryTable.DrawRow($"t-{index + 1}", entries[index].Elapsed.TotalMilliseconds.ToString());
            }

            if (this.options.ShowResults)
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
            if (this.options.Enabled)
                this.DumpOnConsole();
        }
    }
}