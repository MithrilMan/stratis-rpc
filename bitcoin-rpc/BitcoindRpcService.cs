using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace bitcoin_rpc
{
    public class BitcoindRpcService : RpcService
    {
        private Process process;
        private readonly string bitcoinCliPath;
        private readonly string baseArguments;

        public BitcoindRpcService(string friendlyName, string bitcoinCliPath, IPEndPoint rpcEndpoint, string rpcUser, string rpcPassword, string walletPassword, short timeoutInSeconds = 60)
            : base(friendlyName, rpcEndpoint, rpcUser, rpcPassword, walletPassword, timeoutInSeconds)
        {

            this.baseArguments = $"-rpcconnect={rpcEndpoint.Address} -rpcport={rpcEndpoint.Port} rpcuser={rpcUser} -rpcpassword={rpcPassword}"; ;

            process = new Process();
            process.StartInfo.FileName = bitcoinCliPath;
            process.StartInfo.RedirectStandardOutput = true;

            this.bitcoinCliPath = bitcoinCliPath;
        }

        protected override string CallSingleImpl()
        {
            string command = $"getrawtransaction {TestData.GetTxId()} 0";

            string result = this.StartCommand(command);

            return tx?.Hex;
        }

        private string StartCommand(string command)
        {
            process.StartInfo.Arguments = $"{baseArguments} {command}";
            process.Start();

            StringBuilder sb = new StringBuilder();
            while (!process.StandardOutput.EndOfStream)
            {
                sb.AppendLine(process.StandardOutput.ReadLine());
            }
            return sb.ToString();
        }

        protected override string CallBatchImpl(byte[] batchPayload)
        {
            string response = this.service.MakeRawBatchRequests(batchPayload, TimeSpan.FromSeconds(this.timeoutInSeconds));
            return response;
        }
    }
}
