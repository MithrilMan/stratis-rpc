using Newtonsoft.Json;
using StratisRpc.CallRequest;
using StratisRpc.CallResponse;
using StratisRpc.OutputFormatter;
using StratisRpc.Performance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratisRpc.Tests
{
    /// <summary>
    /// Class that holds a node data to be used for tests
    /// </summary>
    public class NodeTestData
    {
        private const int WALLET_UNLOCK_TIMEOUT = 300;

        private readonly IRpcService rpcService;
        private readonly OutputWriter writer;
        private List<string> unspentTxIds;
        private List<string> addresses;
        private List<string> rawUnspentTxHex;
        private Random random;

        public int BlockCount { get; private set; }
        public string BlockHash { get; private set; }

        public int AddressCount => this.addresses?.Count ?? 0;
        public int UtxoCount => this.unspentTxIds?.Count ?? 0;

        public NodeTestData(IRpcService rpcService, OutputWriter writer)
        {
            this.rpcService = rpcService;
            this.writer = writer ?? new OutputWriter();
            this.unspentTxIds = new List<string>();
            this.addresses = new List<string>();
            this.rawUnspentTxHex = new List<string>();
            this.random = new Random();

            this.Initialize();
        }

        private void Initialize()
        {
            const int labelColumnWidth = 30;

            this.writer
                .DrawLine('▼')
                .WriteLine($"Initializing {this.rpcService.Name} Test Data")
                .DrawLine('.');

            this.BlockCount = ParseResponse<int>(new CallRequest.GetBlockCount());
            this.writer.WriteLine($"{"BlockCount".AlignLeft(labelColumnWidth)}{this.BlockCount}");

            this.BlockHash = ParseResponse<string>(new CallRequest.GetBlockHash(this.BlockCount));
            this.writer.WriteLine($"{"BlockHash".AlignLeft(labelColumnWidth)}{this.BlockHash}");

            this.addresses = ParseResponse<ListAddressGroupingsResponse>(new CallRequest.ListAddressGroupings()).AddressesGroups
                .SelectMany(ag => ag)
                .Select(detail => detail.Address)
                .Distinct()
                .ToList();
            this.writer.WriteLine($"{"Loaded Addresses".AlignLeft(labelColumnWidth)}{this.addresses.Count}");


            this.unspentTxIds = ParseResponse<ListUnspentResponse>(new CallRequest.ListUnspent(null, null, null, null, null))
                .Select(unspent => unspent.TxId)
                .ToList();
            this.writer.WriteLine($"{"Loaded UTXOs Ids".AlignLeft(labelColumnWidth)}{this.unspentTxIds.Count}");

            this.rawUnspentTxHex = this.unspentTxIds.Take(20).Select(txId =>
            {
                return ParseResponse<string>(new CallRequest.GetRawTransaction(txId, 0, null));
            }).ToList();
            this.writer.WriteLine($"{"Loaded Raw unspent tx Hex".AlignLeft(labelColumnWidth)}{this.rawUnspentTxHex.Count}");

            this.writer.WriteLine($"Unlocking wallet for {WALLET_UNLOCK_TIMEOUT} seconds");
            this.rpcService.CallSingle(new GenericCall("walletpassphrase", ("passphrase", this.rpcService.WalletPassword), ("timeout", WALLET_UNLOCK_TIMEOUT)));

            this.writer
                .DrawLine('.')
                .WriteLine($"End of {this.rpcService.Name} Test Data Initialization")
                .DrawLine('▲');

        }

        public string GetTxId(int? index = null)
        {
            int maxIndex = this.unspentTxIds.Count;
            index = index ?? random.Next(0, maxIndex + 1);

            return this.unspentTxIds[index.Value % maxIndex];
        }

        public string GetAddress(int? index = null)
        {
            int maxIndex = this.addresses.Count;
            index = index ?? random.Next(0, maxIndex + 1);

            return this.addresses[index.Value % maxIndex];
        }

        public string GetRawHex(int? index = null)
        {
            int maxIndex = this.rawUnspentTxHex.Count;
            index = index ?? random.Next(0, maxIndex + 1);

            return this.rawUnspentTxHex[index.Value % maxIndex];
        }

        private TResult ParseResponse<TResult>(CallRequest.TestRequest request)
        {
            var response = this.rpcService.CallSingle(request);

            if (response.HasError)
            {
                throw new Exception($"Couldn't initialize node data: {response.Error}");
            }

            return JsonConvert.DeserializeObject<RpcResponse<TResult>>(response.Result).Result;
        }
    }
}
