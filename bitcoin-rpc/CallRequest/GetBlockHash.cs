namespace StratisRpc.CallRequest
{
    /// <summary>
    /// getblockhash height
    /// Returns the header hash of a block at the given height in the local best block chain.
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/blockchain/getblockhash/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class GetBlockHash : TestRequest
    {
        public string Height => this.GetParameter<string>(0);

        public GetBlockHash(int height) : base(MethodToTest.GetBlockHash)
        {
            this
                .AddParameter("height", height);
        }
    }
}