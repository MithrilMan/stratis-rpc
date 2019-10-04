using System;
using System.Collections.Generic;
using System.Text;

namespace StratisRpc.CallRequest
{
    public static class TestRequestFactory
    {
        public static TestRequest CreateRequestFor(MethodToTest methodToTest)
        {
            switch (methodToTest)
            {
                case MethodToTest.GetBlockCount:
                    return new GetBlockCount();
                case MethodToTest.GetTransaction:
                    return new GetTransaction(TestData.GetTxId(), null);
                case MethodToTest.GetRawTransaction:
                    return new GetRawTransaction(TestData.GetTxId(), 1, null);
                case MethodToTest.DecodeRawTransaction:
                    return new DecodeRawTransaction(TestData.GetTxId(), null);
                case MethodToTest.ValidateAddress:
                    return new ValidateAddress(TestData.GetAddress());
                case MethodToTest.GetBlockHash:
                    return new GetBlockHash(TestData.BlockHeight);
                case MethodToTest.GetBlock:
                    return new GetBlock(TestData.BlockHash, null);
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
