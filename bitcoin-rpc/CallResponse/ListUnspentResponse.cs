﻿using System.Collections.Generic;

namespace StratisRpc.CallResponse
{
    public class ListUnspentResponse : List<ListUnspentResponse.Details>
    {
        public class Details
        {
            public string TxId { get; set; }
            public int Vout { get; set; }
            public string Address { get; set; }
            public string Account { get; set; }
            public string ScriptPubKey { get; set; }
            public decimal Amount { get; set; }
            public int Confirmations { get; set; }
            public bool Spendable { get; set; }
        }
    }
}
