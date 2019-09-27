using StratisRpc.OutputFormatter;
using StratisRpc.Performance;
using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc
{
    public class TestResult
    {
        public IRpcService Service { get; }
        public int Count { get; }
        public PerformanceEntry PerformanceEntry { get; }

        public TestResult(IRpcService service, int count, PerformanceEntry performanceEntry)
        {
            Service = service;
            Count = count;
            PerformanceEntry = performanceEntry;
        }
    }
}
