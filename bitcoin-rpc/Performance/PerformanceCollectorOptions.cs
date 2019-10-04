using StratisRpc.OutputFormatter;

namespace StratisRpc.Performance
{
    public class PerformanceCollectorOptions
    {
        public static PerformanceCollectorOptions Default = new PerformanceCollectorOptions();
        public static readonly PerformanceCollectorOptions Disabled = new PerformanceCollectorOptions { Enabled = false };

        public bool ShowResponses { get; set; }
        public bool Enabled { get; set; }
        public OutputWriter Writer { get; set; }

        public UnitOfTimeFormatter TimeFormatter { get; set; }

        public PerformanceCollectorOptions()
        {
            this.Enabled = true;
            this.ShowResponses = true;
            this.Writer = null;
            this.TimeFormatter = UnitOfTimeFormatter.Default;
        }
    }
}
