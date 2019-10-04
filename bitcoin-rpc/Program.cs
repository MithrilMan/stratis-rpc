using System;
using System.Net;
using System.Linq;
using StratisRpc.CallRequest;
using StratisRpc.RpcService.RestClient;
using StratisRpc.Performance;
using StratisRpc.Tests;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

namespace StratisRpc
{
    class Program
    {
        public enum VerbosityLevel
        {
            SummaryOnly = 0,
            HideResponses = 1,
            ShowResponses = 2
        }

        static Dictionary<VerbosityLevel, PerformanceCollectorOptions> verbosityLevels = new Dictionary<VerbosityLevel, PerformanceCollectorOptions>
        {
            {VerbosityLevel.SummaryOnly, PerformanceCollectorOptions.Disabled },
            {VerbosityLevel.HideResponses, new PerformanceCollectorOptions { ShowResponses = false }},
            {VerbosityLevel.ShowResponses, new PerformanceCollectorOptions { ShowResponses = true }},
        };
        private static PerformanceCollectorOptions verbosityLevel;

        public class Options
        {
            [JsonConverter(typeof(StringEnumConverter))]
            [Option('v', "verbosity", Required = false, Default = 0, MetaValue = "level", HelpText =
@"Set the verbosity level:
    0 = summary only
    1 = partial timing results
    2 = partial timing results and call responses")]
            public VerbosityLevel Verbosity { get; set; }

            [Option('s', "save", Required = false, MetaValue = "filename", HelpText = "Specify to save the output to the specified filename.")]
            public string FilePath { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            [Option('t', "time-unit", Required = false, Default = 0, HelpText = "Specify the unit to use when displaying time informations. Valid values are 0 (seconds) and 1 (milliseconds)")]
            public UnitOfTimeFormatter.TimeUnit TimeUnit { get; set; }

            [Option('d', "time-decimals", Required = false, Default = 0, HelpText = "Specify the number of decimal digits to include when displaying time informations.")]
            public int TimeDecimals { get; set; }
        }

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            var x = new Parser();

            Parser.Default.ParseArguments<Options>(args)
                .MapResult(options =>
                {
                    if (Enum.IsDefined(typeof(VerbosityLevel), options.Verbosity))
                    {
                        verbosityLevel = verbosityLevels[(VerbosityLevel)options.Verbosity];
                    }
                    else
                    {
                        Console.WriteLine("Invalid verbosity level");
                        return 1;
                    }

                    RunApplication(options);
                    return 0;
                },
                _ => 1);
        }

        private static void RunApplication(Options options)
        {
            UnitOfTimeFormatter.Default.Unit = options.TimeUnit;
            UnitOfTimeFormatter.Default.Decimals = options.TimeDecimals;

            InitializeConnectors();

            Warmup(options);

            if (options.FilePath != null)
            {
                string fileName = Path.GetFullPath(options.FilePath);

                Console.WriteLine($"Results will be saved in {fileName}");
                using (StreamWriter writer = new StreamWriter(fileName) { AutoFlush = true })
                {
                    using (new ConsoleMirroring(writer))
                    {
                        DoTests(options);
                    }
                }
            }
            else
            {
                DoTests(options);
            }
        }

        private static void Warmup(Options options)
        {
            Console.WriteLine("WARMUP");
            TestExecutor.CallBatch(TestRequestFactory.CreateRequestFor(MethodToTest.GetBlockCount), 1, PerformanceCollectorOptions.Disabled);
            Console.WriteLine("END OF WARMUP, don't consider results above.");
            Console.Clear();

            Console.WriteLine($"Using these options: {JsonConvert.SerializeObject(options, Formatting.Indented)}");
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

        private static void DoTests(Options options)
        {
            Console.WriteLine($"Current Time (UTC  ---  Local): {DateTime.UtcNow}  ---  {DateTime.Now}");

            new Tests.Scenarios()
               .Disable()
               .SetOptions(verbosityLevel)
               .CheckAllMethods();


            new Tests.DecodeRawTransaction()
               //.Disable()
               //.SetOptions(verbosityLevels[VerbosityLevel.ShowResponses])
               .SetOptions(verbosityLevel)
               .Execute(10)
               .Batch()
               .Wait();
        }
    }
}
