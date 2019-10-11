using StratisRpc.Tests;
using System;
using System.Collections.Generic;
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
                default:
                    throw new Exception("Unknown method to test.");
            }
        }
    }
}
