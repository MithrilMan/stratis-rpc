using System;
using System.Net;
using System.Linq;
using StratisRpc.CallRequest;
using StratisRpc.RpcService.RestClient;
using StratisRpc.RpcService.Bitcoinlib;
using StratisRpc.RpcService.BitcoinCli;
using StratisRpc.Performance;
using System.Collections.Generic;

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
            CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), 1, false);
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

            rpcServices = new IRpcService[] {
                //new BitcoinlibRpcService("X Node", rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                //new BitcoinlibRpcService("SBFN", rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                //new BitcoinCliRpcService("X Node", settings.bitcoinCliPath, rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                //new BitcoinCliRpcService("SBFN", settings.bitcoinCliPath, rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new RestClientRpcService("X Node", rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new RestClientRpcService("SBFN", rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                new RestClientRpcService("SBFN Local", rpcUrlSbfnLocal, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
            };
        }

        private static void DoTests()
        {
            //CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), 20, false);
            //CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetTransaction), 20, false);
            //CallNTimes(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 20, false);

            using (var collector = new TestResultCollector("GetRawTransaction"))
            {
                CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 1, false, collector);
                CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 10, false, collector);
                CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 15, false, collector);
                CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 30, false, collector);
                CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 60, false, collector);
                CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetRawTransaction), 120, false, collector);
            }

            //using (var collector = new TestResultCollector("GetBlockCount"))
            //{
            //    CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), 120, false, collector);
            //    CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), 240, false, collector);
            //    CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), 480, false, collector);
            //}
        }

        private static void CallNTimes(TestRequest request, int count, bool showPartialResult, TestResultCollector testResultCollector = null)
        {
            var testResults = new List<TestResult>();

            foreach (var service in rpcServices)
            {
                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), showPartialResult))
                {
                    performanceCollector.AddText($"Call {request.MethodToTest} sequentially {count} times\n");

                    for (int _ = 0; _ < count; _++)
                    {
                        PerformanceEntry performance = performanceCollector.Measure(() => service.CallSingle(request));
                        testResults.Add(new TestResult(service, count, performance));
                    }
                }
            }

            if (testResultCollector != null)
            {
                testResultCollector.Collect(count.ToString(), testResults);
            }
            else
            {
                using (testResultCollector = new TestResultCollector(request.MethodToTest.ToString()))
                {
                    testResultCollector.Collect(count.ToString(), testResults);
                }
            }
        }

        private static List<TestResult> CallBatch(TestRequest request, int batchSize, bool showPartialResult, TestResultCollector testResultCollector = null)
        {
            var testResults = new List<TestResult>();

            foreach (var service in rpcServices)
            {
                using (PerformanceCollector performanceCollector = new PerformanceCollector(service.GetServiceDescription(), showPartialResult))
                {
                    performanceCollector.AddText($"Call Batch RPC of { batchSize} {request.MethodToTest} requests.");
                    PerformanceEntry performance = performanceCollector.Measure(() => service.CallBatch(Enumerable.Range(0, batchSize).Select(n => request).ToList()));
                    testResults.Add(new TestResult(service, batchSize, performance));
                }
            }

            if (testResultCollector != null)
            {
                testResultCollector.Collect(batchSize.ToString(), testResults);
            }
            else
            {
                using (testResultCollector = new TestResultCollector(request.MethodToTest.ToString()))
                {
                    testResultCollector.Collect(batchSize.ToString(), testResults);
                }
            }

            return testResults;
        }
    }
}
