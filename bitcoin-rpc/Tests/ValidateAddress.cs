using StratisRpc.CallRequest;
using System.Linq;

namespace StratisRpc.Tests
{
    public class ValidateAddress : TestBase<ValidateAddress>
    {
        public ValidateAddress() : base(MethodToTest.ValidateAddress) { }

        public override ValidateAddress Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, Enumerable.Range(1, 1000).Where(n => n % 50 == 0).ToArray());
        }
    }
}
