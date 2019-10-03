using StratisRpc.CallRequest;
using StratisRpc.Performance;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc.Tests
{
    public abstract class TestBase
    {

        protected void CallNTimes(TestRequest request, int count, bool showPartialResult, TestResultCollector testResultCollector = null)
        {
            TestExecutor.CallNTimes(request, count, showPartialResult, testResultCollector);
        }

        protected List<TestResult> CallBatch(TestRequest request, int batchSize, bool showPartialResult, TestResultCollector testResultCollector = null)
        {
            return TestExecutor.CallBatch(request, batchSize, showPartialResult, testResultCollector);
        }
    }
}