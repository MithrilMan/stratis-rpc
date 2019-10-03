using System;
using System.Net;
using System.Linq;
using StratisRpc.CallRequest;
using StratisRpc.RpcService.RestClient;
using StratisRpc.Performance;
using StratisRpc.Tests;

namespace StratisRpc
{
    class Program
    {

        private static IRpcService[] rpcServices;

        static void Main(string[] args)
        {
            InitializeConnectors();

            Warmup();

            DoTests();

            Console.WriteLine();
        }

        private static void Warmup()
        {
            Console.WriteLine("WARMUP");
            TestExecutor.CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), 1, PerformanceCollectorOptions.Disabled);
            Console.WriteLine("END OF WARMUP, don't consider results above.");
            Console.Clear();
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

            TestExecutor.SetupServices(
                //new BitcoinlibRpcService("X Node", rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                //new BitcoinlibRpcService("SBFN", rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),

                //new BitcoinCliRpcService("X Node", settings.bitcoinCliPath, rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                //new BitcoinCliRpcService("SBFN", settings.bitcoinCliPath, rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),

                new RestClientRpcService("X Node", rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                //new RestClientRpcService("SBFN", rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new RestClientRpcService("SBFN Local", rpcUrlSbfnLocal, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout)
            );
        }

        private static void DoTests()
        {
            PerformanceCollectorOptions options = PerformanceCollectorOptions.Disabled;
            //Tests.GetBalance.Execute(10);
            //Tests.GetTranasction.Batch();

            //Tests.GetRawTransaction.Batch();

            //Tests.GetBlockCount.Execute(20);
            //Tests.GetTransaction.Execute(20);
            //Tests.GetRawTransaction.Execute(20);

            new Tests.ValidateAddress()
                .SetOptions(options)
                .Execute(20)
                .Batch()
                .Wait();

            new Tests.GetBlockHash()
                .SetOptions(options)
                .Execute(20)
                .Batch()
                .Wait();
        }
    }
}
