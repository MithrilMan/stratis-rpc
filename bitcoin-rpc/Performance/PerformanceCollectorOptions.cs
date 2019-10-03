using StratisRpc.OutputFormatter;

namespace StratisRpc.Performance
{
    public class PerformanceCollectorOptions
    {
        public bool ShowResults { get; set; }
        public bool Enabled { get; set; }
        public OutputWriter Writer { get; set; }

        public static PerformanceCollectorOptions Default = new PerformanceCollectorOptions { Enabled = true, ShowResults = false, Writer = null };
        public static readonly PerformanceCollectorOptions Disabled = new PerformanceCollectorOptions { Enabled = false };
    }
}
