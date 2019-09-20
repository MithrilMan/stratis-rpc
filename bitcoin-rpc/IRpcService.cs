using System;

namespace bitcoin_rpc
{
    public interface IRpcService
    {
        string GetServiceDescription();
        void CallBatch(bool showResult, int batchSize, string message = null);
        void CallSingle(bool showResult);
    }
}