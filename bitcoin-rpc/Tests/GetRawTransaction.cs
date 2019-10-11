using StratisRpc.CallRequest;
using System;

namespace StratisRpc.Tests
{
    public class GetRawTransaction : TestBase<GetRawTransaction>
    {
        public GetRawTransaction() : base(MethodToTest.GetRawTransaction) { }

        public override GetRawTransaction Batch(string title = null, bool showResult = false, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, requestFactory, 1, 10, 15, 30, 60, 120);
        }
    }
}
