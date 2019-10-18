namespace StratisRpc.CallRequest
{
    /// <summary>
    /// Send multiple times. Amounts are double-precision floating point numbers
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/wallet/sendmany/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class SendMany : TestRequest
    {
        public string Dummy => this.GetParameter<string>(0);

        /// <summary>
        /// (json object, required) A json object with addresses and amounts
        /// {
        /// "address": amount,    (numeric or string, required) The bitcoin address is the key, the numeric amount(can be string) in BTC is the value
        /// }
        /// </summary>
        public string Amounts => this.GetParameter<string>(1);

        public int? MinConf => this.GetParameter<int?>(2);

        public string Comment => this.GetParameter<string>(3);

        /// <summary>
        /// Gets the subtract fee from.
        /// </summary>
        /// <value>
        /// The subtract fee from.
        /// </value>
        /// (json array, optional) A json array with addresses.
        /// The fee will be equally deducted from the amount of each selected address.
        /// Those recipients will receive less bitcoins than you enter in their corresponding amount field.
        /// If no addresses are specified here, the sender pays the fee.
        /// [
        /// "address", (string)Subtract fee from this address...
        /// ]
        public string SubtractFeeFrom => this.GetParameter<string>(4);

        /// <summary>
        /// (boolean, optional, default=fallback to wallet's default) Allow this transaction to be replaced by a transaction with higher fees via BIP 125
        /// </summary>
        public bool? Replaceable => this.GetParameter<bool?>(5);

        /// <summary>
        /// (numeric, optional, default=fallback to wallet's default) Confirmation target (in blocks)
        /// </summary>
        public int? ConfTarget => this.GetParameter<int?>(6);

        /// <summary>
        /// (string, optional, default=UNSET) The fee estimate mode, must be one of:
        /// "UNSET"
        /// "ECONOMICAL"
        /// "CONSERVATIVE"
        /// </summary>
        public string EstimateMode => this.GetParameter<string>(7);

        public SendMany(string dummy, string amounts, int? minconf, string comment, string subtractfeefrom, bool? replaceable, int? conf_target, string estimate_mode)
            : base(MethodToTest.SendMany)
        {
            this.AddParameter("dummy", dummy);
            this.AddParameter("amounts", amounts);
            this.AddParameter("minconf", minconf);
            this.AddParameter("comment", comment);
            this.AddParameter("subtractfeefrom", subtractfeefrom);
            this.AddParameter("replaceable", replaceable);
            this.AddParameter("conf_target", conf_target);
            this.AddParameter("estimate_mode", estimate_mode);
        }
    }
}
