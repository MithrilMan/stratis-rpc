using BitcoinLib.RPC.RequestResponse;
using Newtonsoft.Json;
using StratisRpc.CallResponse;
using StratisRpc.Performance;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc.Tests
{
    public class SendMany : TestBase<SendMany>
    {
        public class Destination
        {
            public string DestinationAddress { get; }
            public decimal Amount { get; }
            public bool SubtractFees { get; }

            public Destination(string destinationAddress, decimal amount, bool subtractFees)
            {
                DestinationAddress = destinationAddress;
                Amount = amount;
                SubtractFees = subtractFees;
            }
        }

        public SendMany() : base(MethodToTest.SendMany) { }
    }
}
