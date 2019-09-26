using System;
using System.Net;
using System.Linq;
using StratisRpc.CallRequest;
using StratisRpc.RpcService.RestClient;
using StratisRpc.RpcService.Bitcoinlib;
using StratisRpc.RpcService.BitcoinCli;
using StratisRpc.Performance;

namespace StratisRpc
{
    class Program
    {

        private static IRpcService[] rpcServices;

        static void Main(string[] args)
        {
            InitializeConnectors();

            DoTests();

            Console.WriteLine();
        }

        private static void InitializeConnectors()
        {
            IPEndPoint getEndPoint(string hostName, int port)
            {
                IPAddress address = Dns.GetHostAddresses(hostName).First(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                return new IPEndPoint(address, port);
            }

            var settings = new
            {
                rpcPort = 16174,
                rpcUser = "stratis",
                rpcPassword = "node",
                walletPassword = "node",
                timeout = (short)360,
                bitcoinCliPath = @"E:\Sviluppo\InternalTestnet\Util\bitcoin-cli.exe"
            };

            IPEndPoint rpcUrlX = getEndPoint("nodemccx", settings.rpcPort);
            IPEndPoint rpcUrlSbfn = getEndPoint("nodemca", settings.rpcPort);
            IPEndPoint rpcUrlSbfnLocal = getEndPoint("localhost", settings.rpcPort);

            rpcServices = new IRpcService[] {
                new BitcoinlibRpcService("X Node", rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new BitcoinlibRpcService("SBFN", rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new BitcoinCliRpcService("X Node", settings.bitcoinCliPath, rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new BitcoinCliRpcService("SBFN", settings.bitcoinCliPath, rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new RestClientRpcService("X Node", rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new RestClientRpcService("SBFN", rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
            };

            // uncomment line below to filter only RestClient implementation
            rpcServices = rpcServices.OfType<RestClientRpcService>().ToArray();
        }

        private static void DoTests()
        {
            CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), 20, false);
            CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetTransaction), 20, false);
            CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 20, false);

            //CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 1, true);
            //CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 10, false);
            //CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 15, false);
            //CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 30, true);
            //CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 60, false);
            //CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 120, false);
        }

        private static void CallNTimes(TestRequest request, int count, bool showPartialResult)
        {
            foreach (var service in rpcServices)
            {
                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), showPartialResult))
                {
                    performanceCollector.AddText($"Call {request.MethodToTest} sequentially {count} times\n");

                    for (int _ = 0; _ < count; _++)
                    {
                        performanceCollector.Measure(() => service.CallSingle(request));
                    }
                }
            }
        }

        private static void CallBatch(TestRequest request, int batchSize, bool showPartialResult)
        {
            foreach (var service in rpcServices)
            {
                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), showPartialResult))
                {
                    performanceCollector.AddText($"Call Batch RPC of { batchSize} requests");
                    performanceCollector.Measure(() => service.CallBatch(Enumerable.Range(0, batchSize).Select(n => request).ToList()));
                }
            }
        }
    }
}
