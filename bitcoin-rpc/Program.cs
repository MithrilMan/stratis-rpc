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
using StratisRpc.OutputFormatter;

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
            {VerbosityLevel.HideResponses, new PerformanceCollectorOptions { ShowResponses = false, Writer = new OutputWriter() }},
            {VerbosityLevel.ShowResponses, new PerformanceCollectorOptions { ShowResponses = true, Writer = new OutputWriter() }},
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

            [Option('k', "import-keys", Required = false, MetaValue = "filename", HelpText = "Specify the path to the file containing keys to be imported on an X node.")]
            public string ImportKeysFromFile { get; set; }
        }

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            PerformanceCollectorOptions.Default.Writer = PerformanceCollectorOptions.Disabled.Writer = new OutputWriter();

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

                    Console.WriteLine($"Using these options: {JsonConvert.SerializeObject(options, Formatting.Indented)}");

                    RunApplication(options);
                    return 0;
                },
                _ => 1);
        }

        private static void RunApplication(Options options)
        {
            UnitOfTimeFormatter.Default.Unit = options.TimeUnit;
            UnitOfTimeFormatter.Default.Decimals = options.TimeDecimals;


            if (options.FilePath != null)
            {
                string fileName = Path.GetFullPath(options.FilePath);

                Console.WriteLine($"Results will be saved in {fileName}");
                using (StreamWriter writer = new StreamWriter(fileName) { AutoFlush = true })
                {
                    using (new ConsoleMirroring(writer))
                    {
                        InitializeConnectors();
                        DoTests(options);
                    }
                }
            }
            else
            {
                InitializeConnectors();
                DoTests(options);
            }
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
            IPEndPoint rpcUrlSbfn = getEndPoint("nodemaa", settings.rpcPort);
            IPEndPoint rpcUrlSbfnLocal = getEndPoint("localhost", settings.rpcPort);

            TestExecutor.SetupServices(new OutputWriter(),
                // new RestClientRpcService("X Node", rpcUrlX, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),
                //new RestClientRpcService("SBFN", rpcUrlSbfn, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout),

                new RestClientRpcService("X Node Docker", getEndPoint("nodemccx", settings.rpcPort), settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout)
                //new RestClientRpcService("X Node Local", getEndPoint("localhost", 16174), settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout)

            //new RestClientRpcService("SBFN Docker", getEndPoint("nodemaa", settings.rpcPort), settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout)
            //new RestClientRpcService("SBFN Local", rpcUrlSbfnLocal, settings.rpcUser, settings.rpcPassword, settings.walletPassword, settings.timeout)
            );
        }

        private static void DoTests(Options options)
        {
            Console.WriteLine($"Current Time (UTC  ---  Local): {DateTime.UtcNow}  ---  {DateTime.Now}");

            if (options.ImportKeysFromFile != null)
            {
                ImportKeys(TestExecutor.Services.FirstOrDefault(service => service.Name.Contains("X")), options.ImportKeysFromFile);
                return;
            }

            new Tests.Scenarios()
               .Disable()
               .SetOptions(verbosityLevel)
               .CheckAllMethods(false)
               //.TestSendMany(10, 1)
               //.TestSendMany(100, 1)
               //.TestSendMany(10, 100)
               //.TestSendMany(100, 100)
               ;

            new Tests.GetBalance()
               .Disable()
               .SetOptions(verbosityLevel)
               .SetOptions(verbosityLevels[VerbosityLevel.ShowResponses])
               .Execute(1)
               ;

            new Tests.SendMany()
               .Disable()
               .SetOptions(verbosityLevel)
               .SetOptions(verbosityLevels[VerbosityLevel.ShowResponses])
               .Execute(1)
               ;

            new Tests.DecodeRawTransaction()
               .Disable()
               .SetOptions(verbosityLevel)
               .SetOptions(verbosityLevels[VerbosityLevel.ShowResponses])
               .Execute(10)
               //.Batch(null, null, 1000)
               ;

            //new Tests.GetTransaction()
            //   .Disable()
            //   .SetOptions(verbosityLevel)
            //   //.SetOptions(verbosityLevels[VerbosityLevel.ShowResponses])
            //   //.Execute(10)
            //   //.Batch()
            //   .GetSpecificTransaction("8fd79542c5c3291fff653050b7ee36692d29290d62e1069089a9cd95a1c0be1b", 5)
            //   .GetSpecificTransaction("9fd79542c5c3291fff653050b7ee36692d29290d62e1069089a9cd95a1c0be1b", 5)
            //   .Wait();

            TestExecutor.DumpSummary(verbosityLevel.Writer, verbosityLevel.TimeFormatter);
        }


        internal class ImportedKey
        {
            public string Address { get; set; }
            public string PrivateKey { get; set; }
            public string Path { get; set; }
        }

        private static void ImportKeys(IRpcService service, string privateKeysFilePath)
        {
            verbosityLevel.Writer.WriteLine($"Importing private keys");
            if (service == null)
            {
                verbosityLevel.Writer.WriteLine("No X node found to import keys into.");
                return;
            }

            if (!File.Exists(privateKeysFilePath))
            {
                verbosityLevel.Writer.WriteLine($"{privateKeysFilePath} file not found.");
            }

            var keysToImport = JsonConvert.DeserializeObject<List<ImportedKey>>(File.ReadAllText(privateKeysFilePath));

            verbosityLevel.Writer.WriteLine($"Found {keysToImport.Count} private keys to import.");

            for (int i = 0; i < keysToImport.Count; i++)
            {
                var keyToImport = keysToImport[i];

                var request = new GenericCall(
                    "importprivkey",
                    ("privkey", keyToImport.PrivateKey),
                    ("label", keyToImport.Path),
                    ("rescan", false) //i == keysToImport.Count - 1)
                    );

                var result = service.CallSingle(request);

                Console.Write($"\rImporting key {i + 1}/{keysToImport.Count}");
                if (result.HasError)
                    verbosityLevel.Writer.WriteLine($"\rimportprivkey error on key {i + 1}/{keysToImport.Count}: {result.Error}");
            }

            verbosityLevel.Writer.WriteLine($"Check if first key has been imported");
            var dumpPrivKeyResult = service.CallSingle(new GenericCall("dumpprivkey", ("address", keysToImport.First().Address)));
            if (dumpPrivKeyResult.HasError)
                verbosityLevel.Writer.WriteLine($"dumpprivkey error: {dumpPrivKeyResult.Error}");
            else
                verbosityLevel.Writer.WriteLine($"dumpprivkey result: {dumpPrivKeyResult.Result}");
        }
    }
}
