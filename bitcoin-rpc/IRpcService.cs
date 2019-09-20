using System;

namespace StratisRpc
{
    public interface IRpcService
    {
        string GetServiceDescription();
        void CallBatch(bool showResult, int batchSize, string message = null);
        void CallSingle(bool showResult);
    }
}