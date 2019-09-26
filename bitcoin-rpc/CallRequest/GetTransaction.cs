using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.CallRequest
{
    /// <summary>
    /// gettransaction "txid" ( include_watchonly )
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/wallet/gettransaction/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class GetTransaction : TestRequest
    {
        public string TxId => this.GetParameter<string>(0);

        public bool? IncludeWatchOnly => this.GetParameter<bool?>(1);

        public GetTransaction(string txId, bool? includeWatchOnly) : base(MethodToTest.GetTransaction)
        {
            this
                .AddParameter("txId", txId)
                .AddParameter("includeWatchOnly", includeWatchOnly);
        }
    }
}
