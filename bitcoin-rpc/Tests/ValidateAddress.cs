using StratisRpc.CallRequest;
using System.Linq;

namespace StratisRpc.Tests
{
    public class ValidateAddress : TestBase<ValidateAddress>
    {
        public ValidateAddress() : base(MethodToTest.ValidateAddress) { }

        public override ValidateAddress Batch(string title = null, bool showResult = false, TestRequest request = null, params int[] batchSizes)
        {
            return base.Batch(title, showResult, request, Range(0, 1000, 50).ToArray());
        }
    }
}
