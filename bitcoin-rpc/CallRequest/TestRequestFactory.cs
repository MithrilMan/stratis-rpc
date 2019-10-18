using Newtonsoft.Json;
using StratisRpc.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StratisRpc.CallRequest
{
    public static class TestRequestFactory
    {
        public static TestRequest CreateRequestFor(MethodToTest methodToTest, NodeTestData testData)
        {
            switch (methodToTest)
            {
                case MethodToTest.GetBlockCount:
                    return new GetBlockCount();
                case MethodToTest.GetTransaction:
                    return new GetTransaction(testData.GetTxId(), null);
                case MethodToTest.GetRawTransaction:
                    return new GetRawTransaction(testData.GetTxId(), 1, null);
                case MethodToTest.DecodeRawTransaction:
                    return new DecodeRawTransaction(testData.GetRawHex(), null);
                case MethodToTest.ValidateAddress:
                    return new ValidateAddress(testData.GetAddress());
                case MethodToTest.GetBlockHash:
                    return new GetBlockHash(testData.BlockCount);
                case MethodToTest.GetBlock:
                    return new GetBlock(testData.BlockHash, null);
                case MethodToTest.GetBalance:
                    return new GetBalance(null, null, null);
                case MethodToTest.ListUnspent:
                    return new ListUnspent(null, null, null, null, null);
                case MethodToTest.ListAddressGroupings:
                    return new ListAddressGroupings();
                case MethodToTest.SendMany:
                    List<Tests.SendMany.Destination> destinations = new List<Tests.SendMany.Destination>{
                        new Tests.SendMany.Destination(testData.GetAddress(0), 1, false),
                        new Tests.SendMany.Destination(testData.GetAddress(1), 1, true),
                        new Tests.SendMany.Destination(testData.GetAddress(2), 1, true),
                    };
                    var amounts = destinations.ToDictionary(d=>d.DestinationAddress, d=>d.Amount);
                    string jsonAmounts = JsonConvert.SerializeObject(amounts);

                    var subtractFees = destinations.Where(dest => dest.SubtractFees).Select(dest => dest.DestinationAddress).Distinct().ToArray();
                    string jsonSubtractFees = JsonConvert.SerializeObject(subtractFees);

                    CallRequest.SendMany request = new CallRequest.SendMany(string.Empty, jsonAmounts, null, null, jsonSubtractFees, null, null, "UNSET");
                    return request;
                default:
                    throw new Exception("Unknown method to test.");
            }
        }
    }
}
