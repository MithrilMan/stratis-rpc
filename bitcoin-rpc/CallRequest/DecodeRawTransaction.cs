using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.CallRequest
{
    /// <summary>
    /// decoderawtransaction "hexstring" ( iswitness )
    /// Return a JSON object representing the serialized, hex-encoded transaction.
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/rawtransactions/decoderawtransaction/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class DecodeRawTransaction : TestRequest
    {
        public string HexString => this.GetParameter<string>(0);

        public bool IsWitness => this.GetParameter<bool>(1);

        public DecodeRawTransaction(string hexstring, bool? iswitness) : base(MethodToTest.DecodeRawTransaction)
        {
            this.AddParameter("hexstring", hexstring)
                .AddParameter("iswitness", iswitness);
        }
    }
}
