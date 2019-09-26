using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.CallRequest
{
    /// <summary>
    /// getrawtransaction "txid" ( verbose "blockhash" )
    /// Return the raw transaction data.
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/rawtransactions/getrawtransaction/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class GetRawTransaction : TestRequest
    {
        public string TxId => this.GetParameter<string>(0);

        public int? Verbose => this.GetParameter<int?>(1);

        public string BlockHash => this.GetParameter<string>(2);


        public GetRawTransaction(string txId, int? verbose, string blockhash) : base(MethodToTest.GetRawTransaction)
        {
            this.AddParameter("txId", txId)
                .AddParameter("verbose", verbose)
                .AddParameter("blockhash", blockhash);
        }
    }
}
