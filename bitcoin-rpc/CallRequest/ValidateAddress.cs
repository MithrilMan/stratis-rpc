namespace StratisRpc.CallRequest
{
    /// <summary>
    /// validateaddress "address"
    /// Returns information about the given Bitcoin address.
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/util/validateaddress/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class ValidateAddress : TestRequest
    {
        public string Address => this.GetParameter<string>(0);

        public ValidateAddress(string address) : base(MethodToTest.ValidateAddress)
        {
            this.AddParameter("address", address);
        }
    }
}
