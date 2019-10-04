using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.CallRequest
{
    /// <summary>
    /// listunspent ( minconf maxconf ["address",...] include_unsafe query_options )
    /// Returns array of unspent transaction outputs with between minconf and maxconf(inclusive) confirmations.
    /// Optionally filter to only include txouts paid to specified addresses.
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/wallet/listunspent/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class ListUnspent : TestRequest
    {

        public int? MinConf => this.GetParameter<int?>(0);

        public int? MaxConf => this.GetParameter<int?>(1);

        public string[] Addresses => this.GetParameter<string[]>(2);

        public bool? IncludeUnsafe => this.GetParameter<bool?>(3);

        public string QueryOptions => this.GetParameter<string>(4);

        public ListUnspent(int? minconf, int? maxconf, string[] addresses, bool? include_unsafe, string query_options) : base(MethodToTest.ListUnspent)
        {
            this
                .AddParameter("minconf", minconf)
                .AddParameter("maxconf", maxconf)
                .AddParameter("addresses", addresses)
                .AddParameter("include_unsafe", include_unsafe)
                .AddParameter("query_options", query_options);
        }
    }
}
