using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.CallRequest
{
    /// <summary>
    /// getbalance  ("dummy" minconf include_watchonly )
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/wallet/getbalance/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class GetBalance : TestRequest
    {
        public string Dummy => this.GetParameter<string>(0);

        public int? MinConf => this.GetParameter<int?>(1);
        public bool? IncludeWatchOnly => this.GetParameter<bool?>(1);

        public GetBalance(string dummy, int? minconf, bool? includeWatchOnly) : base(MethodToTest.GetBalance)
        {
            this
                .AddParameter("dummy", dummy)
                .AddParameter("minconf", minconf)
                .AddParameter("includeWatchOnly", includeWatchOnly);
        }
    }
}
