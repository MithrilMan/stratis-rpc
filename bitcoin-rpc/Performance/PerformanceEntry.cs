using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.Performance
{
    public class PerformanceEntry
    {
        public TimeSpan Elapsed { get; }
        public string Result { get; }

        public bool HasError { get; }

        public string Error { get; }

        public PerformanceEntry(TimeSpan elapsed, string result)
        {
            Elapsed = elapsed;
            Result = result;

            JToken jsonResult = JToken.Parse(result);

            JToken error = null;
            if (jsonResult is JArray asArray)
            {
                error = asArray[0]["error"];
            }
            else if (jsonResult is JObject asObject)
            {
                error = asObject["error"];
            }

            this.HasError = error.HasValues;
            if (this.HasError)
            {
                this.Error = error.ToString();
            }
        }
    }
}
