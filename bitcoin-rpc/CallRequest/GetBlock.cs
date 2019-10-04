namespace StratisRpc.CallRequest
{
    /// <summary>
    /// getblock "blockhash" ( verbosity )
    /// If verbosity is 0, returns a string that is serialized, hex-encoded data for block 'hash'.
    /// If verbosity is 1, returns an Object with information about block<hash>.
    /// If verbosity is 2, returns an Object with information about block<hash> and information about each transaction.
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/blockchain/getblock/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class GetBlock : TestRequest
    {
        public string BlockHash => this.GetParameter<string>(0);
        public int? Verbosity => this.GetParameter<int?>(1);

        public GetBlock(string blockHash, int? verbosity) : base(MethodToTest.GetBlock)
        {
            this
                .AddParameter("blockhash", blockHash)
                .AddParameter("verbosity", verbosity);
        }
    }
}