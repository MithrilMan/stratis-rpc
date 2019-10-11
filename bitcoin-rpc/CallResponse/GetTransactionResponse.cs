using System.Collections.Generic;

namespace StratisRpc.CallResponse
{
    public class GetTransactionResponse
    {
        public class TransactionDetails
        {
            public string Account { get; set; }
            public string Address { get; set; }
            public decimal Amount { get; set; }
            public string Label { get; set; }
            public decimal Fee { get; set; }
            public int Vout { get; set; }
            public string Category { get; set; }
        }

        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string BlockHash { get; set; }
        public int BlockIndex { get; set; }
        public int BlockTime { get; set; }
        public int Confirmations { get; set; }
        public List<TransactionDetails> Details { get; set; }
        public string Hex { get; set; }
        public int Time { get; set; }
        public int TimeReceived { get; set; }
        public List<string> WalletConflicts { get; set; }
        public string TxId { get; set; }
    }
}
