using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StratisRpc.Performance
{
    public class PerformanceCollector : IDisposable
    {
        const int ConsoleWidth = 80;

        /// <summary>
        /// Produces a text.
        /// </summary>
        /// <param name="text">The text.</param>
        public delegate void Writer(string text);

        private static SummarySettings summarySettings = new SummarySettings();
        private readonly bool showCallResult;
        private List<PerformanceEntry> entries;
        private Writer writer;

        public string Context { get; }


        public PerformanceCollector(string context, bool showCallResult, Writer writer = null)
        {
            this.Context = context ?? String.Empty;
            this.showCallResult = showCallResult;
            this.writer = writer ?? Console.Write;
            this.entries = new List<PerformanceEntry>();

            WriteLine(String.Empty);
            WriteLine(DrawLine('='));
            WriteLine(Center(context));
            WriteLine(DrawLine('='));
        }


        protected void AddResult(PerformanceEntry entry)
        {
            this.entries.Add(entry);
            Write($"! ");
        }

        public void AddText(string text)
        {
            WriteLine(text);
        }


        /// <summary>
        /// Measures the specified action logging its PerformanceEntry.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public PerformanceCollector Measure(Func<PerformanceEntry> action)
        {
            Write($"t-{this.entries.Count + 1}...");

            PerformanceEntry entry = action();
            this.AddResult(entry);

            return this;
        }

        /// <summary>
        /// Dumps the summary of the performance entries on console.
        /// </summary>
        public void DumpOnConsole()
        {
            WriteLine();
            WriteLine();
            WriteLine(DrawLine('-', summarySettings.Width));
            WriteLine($"{Align("Index", summarySettings.Index)}{summarySettings.ColumnSeparator}{Align("Elapsed (ms)", summarySettings.Elapsed)}{summarySettings.ColumnSeparator}");
            WriteLine(DrawLine('-', summarySettings.Width));

            for (int index = 0; index < entries.Count; index++)
            {
                PerformanceEntry entry = entries[index];
                WriteLine($"{Align($"t-{index}", summarySettings.Index)} | {Align(entry.Elapsed.TotalMilliseconds.ToString(), summarySettings.Elapsed)} |");
            }

            WriteLine(DrawLine('-', summarySettings.Width));

            if (this.showCallResult)
            {
                WriteLine();
                WriteLine(DrawLine('*'));
                WriteLine(Center("RESULTS", '*'));
                for (int index = 0; index < entries.Count; index++)
                {
                    PerformanceEntry entry = entries[index];
                    WriteLine($"t-{index}: { entry.Result}");
                }
                WriteLine(DrawLine('*'));
                WriteLine();
            }

            DrawLine('=');
            DrawLine('=');
            WriteLine();
        }

        #region Text Drawing
        private static string DrawLine(char character = '-', int lenght = ConsoleWidth)
        {
            return string.Empty.PadRight(lenght, character);
        }

        private static string Align(string text, int totalWidth = ConsoleWidth, char paddingCharacter = ' ')
        {
            return totalWidth > 0 ? text.PadRight(totalWidth, paddingCharacter) : text.PadLeft(Math.Abs(totalWidth), paddingCharacter);
        }

        private static string Center(string text, int totalWidth = ConsoleWidth, char paddingCharacter = ' ')
        {
            return text.PadLeft((totalWidth + text.Length) / 2, paddingCharacter);
        }

        private void Write(string text)
        {
            this.writer(text);
        }

        private void WriteLine(string text = null)
        {
            this.writer(text + '\n');
        }
        #endregion

        public void Dispose()
        {
            this.DumpOnConsole();
        }

        private sealed class SummarySettings
        {
            public int Index = 8;
            public int Elapsed = -20;
            public string ColumnSeparator = " | ";
            public readonly int Width;

            public SummarySettings()
            {
                this.Width = Math.Abs(this.Index) + ColumnSeparator.Length + Math.Abs(this.Elapsed) + ColumnSeparator.Length;
            }

        }
    }
}