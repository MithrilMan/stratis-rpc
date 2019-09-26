using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.CallRequest
{
    /// <summary>
    /// getblockcount
    /// Returns the number of blocks in the longest blockchain.
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/blockchain/getblockcount/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class GetBlockCount : TestRequest
    {
        public GetBlockCount() : base(MethodToTest.GetBlockCount)
        {

        }
    }
}
