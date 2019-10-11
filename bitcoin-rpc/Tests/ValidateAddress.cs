using StratisRpc.CallRequest;
using System;
using System.Linq;

namespace StratisRpc.Tests
{
    public class ValidateAddress : TestBase<ValidateAddress>
    {
        public ValidateAddress() : base(MethodToTest.ValidateAddress) { }

        public override ValidateAddress Batch(string title = null, Func<IRpcService, TestRequest> requestFactory = null, params int[] batchSizes)
        {
            if (batchSizes.Length == 0) batchSizes = this.Range(0, 1000, 50).ToArray();

            return base.Batch(title, requestFactory, batchSizes);
        }
    }
}
