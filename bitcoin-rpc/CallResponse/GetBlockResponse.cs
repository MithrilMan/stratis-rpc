namespace StratisRpc.CallResponse
{
    public class GetBlockResponse
    {
        public string Hash { get; set; }
        public int Confirmations { get; set; }
        public int Size { get; set; }
        public int StrippedSize { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public int Version { get; set; }
        public string VersionHex { get; set; }
        public string MerkleRoot { get; set; }
        public string[] Tx { get; set; }
        public long Time { get; set; }
        public long MedianTime { get; set; }
        public long Nonce { get; set; }
        public string Bits { get; set; }
        public decimal Difficulty { get; set; }
        public string ChainWork { get; set; }
        public int NTx { get; set; }
        public string PreviousBlockHash { get; set; }
        public string NextBlockHash { get; set; }
    }
}
