using StratisRpc.Tests;
using System;
using System.Collections.Generic;

namespace StratisRpc
{
    public class TestSummary
    {
        private readonly NodeTestData nodeTestData;

        public class Detail
        {
            public string CalledMethod { get; }
            public TimeSpan Elapsed { get; }

            public Detail(string calledMethod, TimeSpan elapsed)
            {
                this.CalledMethod = calledMethod;
                this.Elapsed = elapsed;
            }
        }

        public class BatchDetail : Detail
        {
            public int BatchSize { get; }

            public BatchDetail(string calledMethod, TimeSpan elapsed, int batchSize) : base(calledMethod, elapsed)
            {
                BatchSize = batchSize;
            }

        }

        public int UtxoCount => nodeTestData.UtxoCount;
        public int AddressCount => nodeTestData.AddressCount;
        public int BlockCount => nodeTestData.BlockCount;

        /// <summary>
        /// Gets or sets the details of a tested method (key is the tested method name)
        /// </summary>
        public Dictionary<string, List<Detail>> Details { get; set; }

        public TestSummary(NodeTestData nodeTestData)
        {
            this.nodeTestData = nodeTestData;
            this.Details = new Dictionary<string, List<Detail>>();
        }

        public void RegisterSingleCallResult(string calledMethod, TimeSpan elapsed)
        {
            string key = calledMethod;
            RegisterResult(key, new Detail(calledMethod, elapsed));
        }

        public void RegisterBatchCallResult(string calledMethod, int batchSize, TimeSpan elapsed)
        {
            string key = $"{calledMethod} - batch({batchSize})";
            RegisterResult(key, new BatchDetail(calledMethod, elapsed, batchSize));
        }

        private void RegisterResult(string key, Detail newDetail)
        {
            if (!this.Details.TryGetValue(key, out List<Detail> details))
            {
                details = new List<Detail>();
                this.Details[key] = details;
            }

            details.Add(newDetail);
        }
    }
}
