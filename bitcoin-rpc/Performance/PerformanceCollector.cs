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
            this.options = options ?? new PerformanceCollectorOptions { Enabled = true, ShowResponses = false };
            this.writer = options.Writer ?? new OutputWriter();
            this.entries = new List<PerformanceEntry>();

            this.summaryTable = new TableBuilder(this.writer)
                .AddColumn(new ColumnDefinition { Label = "Call N.", Width = 8, Alignment = ColumnAlignment.Left })
                .AddColumn(new ColumnDefinition { Label = "Elapsed", Width = 20, Alignment = ColumnAlignment.Right })
                .AddColumn(new ColumnDefinition { Label = "Error", Width = 5, Alignment = ColumnAlignment.Center })
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
                this.summaryTable.DrawRow($"t-{entries.Count}", this.options.TimeFormatter.Format(entries[entries.Count - 1].Elapsed), entry.HasError ? "*" : String.Empty);
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
            //start drawing the table header if it's the first measure
            if (this.entries.Count == 0 && options.Enabled)
                this.summaryTable.Start();

            PerformanceEntry entry = action();
            this.AddResult(entry);

            return entry;
        }

        /// <summary>
        /// Dumps the summary of the performance entries on console.
        /// </summary>
        private void DumpResultsAndEnd()
        {
            if (!options.Enabled)
                return;

            if (this.options.ShowResponses)
            {
                this.writer.WriteLine();
                this.writer.WriteLine("RESPONSES".Center(80, '*'));
                for (int index = 0; index < entries.Count; index++)
                {
                    PerformanceEntry entry = entries[index];
                    this.writer.WriteLine($"t-{index}: { entry.Result}");
                }
                this.writer.DrawLine('*');
                this.writer.WriteLine();
            }

            this.writer.DrawLine('=');
            this.writer.WriteLine();
        }

        public void Dispose()
        {
            if (this.options.Enabled)
                this.DumpResultsAndEnd();
        }
    }
}