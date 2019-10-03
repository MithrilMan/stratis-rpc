using StratisRpc.CallRequest;

namespace StratisRpc.Tests
{
    public class GetBalance : TestBase<GetBalance>
    {
        public GetBalance() : base(MethodToTest.GetBalance) { }
    }
}
