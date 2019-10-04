using StratisRpc.OutputFormatter;

namespace StratisRpc.Performance
{
    public class PerformanceCollectorOptions
    {
        public bool ShowResponses { get; set; }
        public bool Enabled { get; set; }
        public OutputWriter Writer { get; set; }

        public static PerformanceCollectorOptions Default = new PerformanceCollectorOptions();
        public static readonly PerformanceCollectorOptions Disabled = new PerformanceCollectorOptions { Enabled = false };

        public PerformanceCollectorOptions()
        {
            this.Enabled = true;
            this.ShowResponses = true;
            this.Writer = null;
        }
    }
}
