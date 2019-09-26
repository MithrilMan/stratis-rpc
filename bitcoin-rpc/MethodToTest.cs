namespace StratisRpc
{
    /// <summary>
    /// We need all of those RPC calls:
    /// GetBlockCount                       ------NOPROB
    /// GetTransaction **W                  ------NOPROB
    /// GetRawTransaction
    /// DecodeRawTransaction
    /// ValidateAddress
    /// GetBlockHash
    /// GetBlock
    /// GetBalance **W
    /// ListUnspent **W
    /// ListAddressGroupings **W
    /// SendMany **W
    /// </summary>
    public enum MethodToTest
    {
        GetBlockCount,
        GetTransaction,
        GetRawTransaction,
        DecodeRawTransaction,
        ValidateAddress,
        GetBlockHash,
        GetBlock,
        GetBalance,
        ListUnspent,
        ListAddressGroupings,
        SendMany
    }
}
