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
                case MethodToTest.GetBlock:
                case MethodToTest.GetBalance:
                    return new GetBalance(null, null, null);
                case MethodToTest.ListUnspent:
                case MethodToTest.ListAddressGroupings:
                case MethodToTest.SendMany:
                default:
                    throw new Exception("Unknown method to test.");
            }
        }
    }
}
