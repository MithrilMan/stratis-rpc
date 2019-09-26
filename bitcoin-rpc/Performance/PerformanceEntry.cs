using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.Performance
{
    public class PerformanceEntry
    {
        public TimeSpan Elapsed { get; }
        public string Result { get; }

        public PerformanceEntry(TimeSpan elapsed, string result)
        {
            Elapsed = elapsed;
            Result = result;
        }
    }
}
