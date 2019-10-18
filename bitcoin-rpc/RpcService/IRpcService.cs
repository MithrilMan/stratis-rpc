using StratisRpc.CallRequest;
using StratisRpc.Performance;
using System;
using System.Collections.Generic;

namespace StratisRpc
{
    public interface IRpcService
    {
        string Name { get; }

        string WalletPassword { get; }

        string GetServiceDescription();

        /// <summary>
        /// Calls a single request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The elapsed time and the call result.
        /// </returns>
        PerformanceEntry CallSingle(TestRequest request);

        /// <summary>
        /// Calls the batch of requests.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>
        /// The elapsed time and the call result.
        /// </returns>
        PerformanceEntry CallBatch(List<TestRequest> requests);
    }
}