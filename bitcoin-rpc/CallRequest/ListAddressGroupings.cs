using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.CallRequest
{
    /// <summary>
    /// listaddressgroupings
    /// Returns a lists groups of addresses which have had their common ownership
    /// made public by common use as inputs or as the resulting change in past transactions.
    /// https://bitcoincore.org/en/doc/0.18.0/rpc/wallet/listaddressgroupings/
    /// </summary>
    /// <seealso cref="StratisRpc.CallRequest.TestRequest" />
    public class ListAddressGroupings : TestRequest
    {
        public ListAddressGroupings() : base(MethodToTest.ListAddressGroupings)
        {
        }
    }
}
