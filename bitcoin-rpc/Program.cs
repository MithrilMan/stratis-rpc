using System;
using System.Diagnostics;

namespace bitcoin_rpc
{
    class Program
    {
        private const string API_URL = "http://localhost:37221/api";

        private const string RPC_URL_X = "http://nodemccx:16174";
        private const string RPC_URL_SBFN = "http://localhost:16174";
        private static Stopwatch sw = new Stopwatch();

        private static BitcoinlibRpcService bitcoinlibRpcServiceX = new BitcoinlibRpcService("X Node", RPC_URL_X, "stratis", "node", "node", 60);
        private static BitcoinlibRpcService bitcoinlibRpcServiceSbfn = new BitcoinlibRpcService("SBFN", RPC_URL_SBFN, "stratis", "node", "node", 60);
        private static RestClientRpcService restClientX = new RestClientRpcService("X Node", RPC_URL_X, "stratis", "node", "node", 60);
        private static RestClientRpcService restClientSbfn = new RestClientRpcService("SBFN", RPC_URL_SBFN, "stratis", "node", "node", 60);

        private static IRpcService[] rpcServices =
        {
            bitcoinlibRpcServiceX,
            bitcoinlibRpcServiceSbfn,
            restClientX,
            restClientSbfn
        };

        static void Main(string[] args)
        {
            foreach (var service in rpcServices)
            {
                CallNTimes(service, 5, false);
            }

            foreach (var service in rpcServices)
            {
                CallBatch(service, 10, false);
            }
            //CallBatch(bitcoinlibRpcServiceSbfn, 10, false);
            //CallBatch(bitcoinlibRpcServiceSbfn, 10, false);
            //CallBatch(bitcoinlibRpcServiceSbfn, 10, false);
            //CallBatch(bitcoinlibRpcServiceSbfn, 10, false);


            //CallBatch(sw, false, 5);
            //CallBatch(sw, false, 10);
            //CallBatch(sw, false, 20);
            //CallBatch(sw, false, 40);

            //Console.WriteLine("Call API endpoint 5 times synchronously");
            //CallApi(sw, true);
            //CallApi(sw, true);
            //CallApi(sw, true);
            //CallApi(sw, true);
            //CallApi(sw, true);

            //Console.WriteLine("Call API endpoint 5 times asynchronously");
            //CallMultipleApiAsync(sw, true, 10);


            //CallMultiple(sw, 3);

            Console.WriteLine();
        }

        private static void CallBatch(IRpcService service, int batchSize, bool showResult)
        {
            service.TestThis($"Call Batch RPC", () =>
            {
                service.CallBatch(showResult, batchSize);
            });
        }

        private static void CallNTimes(IRpcService service, int count, bool showResult)
        {
            service.TestThis($"Call sequentially {count} times ", () =>
            {
                for (int _ = 0; _ < count; _++)
                {
                    service.CallSingle(showResult);
                }
            });
        }
    }
}
