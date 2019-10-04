using System;
using System.Net;
using System.Linq;
using StratisRpc.CallRequest;
using StratisRpc.RpcService.RestClient;
using StratisRpc.Performance;
using StratisRpc.Tests;
using System.IO;

namespace StratisRpc
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeConnectors();

            Warmup();

            if (args.Contains("--save"))
            {
                string fileName = Path.GetFullPath("./results.txt");

                Console.WriteLine($"Results will be saved on {fileName}");
                using (StreamWriter writer = new StreamWriter(fileName) { AutoFlush = true })
                {
                    using (new ConsoleMirroring(writer))
                    {
                        DoTests();
                    }
                }
            }
            else
            {
                DoTests();
            }
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
                timeout = (short)360, //seconds
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
            PerformanceCollectorOptions summaryOnly = PerformanceCollectorOptions.Disabled;
            PerformanceCollectorOptions showResponses = new PerformanceCollectorOptions { ShowResponses = true };
            PerformanceCollectorOptions hideResponses = new PerformanceCollectorOptions { ShowResponses = false };

            Console.WriteLine($"Current Time (UTC/Local): {DateTime.UtcNow}/{DateTime.Now}");

            new Tests.Scenarios()
               //.Disable()
               .SetOptions(hideResponses)
               .CheckAllMethods();


            new Tests.ListAddressGroupings()
               .Disable()
               .SetOptions(hideResponses)
               .Execute(1)
               //.Batch()
               .Wait();
        }
    }
}
