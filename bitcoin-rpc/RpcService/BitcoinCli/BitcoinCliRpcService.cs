using StratisRpc.CallRequest;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace StratisRpc.RpcService.BitcoinCli
{
    public class BitcoinCliRpcService : RpcServiceBase
    {
        private readonly string bitcoinCliPath;
        private readonly string baseArguments;

        public BitcoinCliRpcService(string friendlyName, string bitcoinCliPath, IPEndPoint rpcEndpoint, string rpcUser, string rpcPassword, string walletPassword, short timeoutInSeconds = 60)
            : base(friendlyName, rpcEndpoint, rpcUser, rpcPassword, walletPassword, timeoutInSeconds)
        {

            this.baseArguments = $"-rpcconnect={rpcEndpoint.Address} -rpcport={rpcEndpoint.Port} rpcuser={rpcUser} -rpcpassword={rpcPassword}"; ;

            this.bitcoinCliPath = bitcoinCliPath;
        }

        protected override string CallSingleImpl(TestRequest request)
        {
            string command = request.ToString();

            string result = this.StartCommand(command);

            return result;
        }

        private string StartCommand(string command)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = bitcoinCliPath;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Arguments = $"{baseArguments} {command}";
                process.Start();

                StringBuilder sb = new StringBuilder();
                while (!process.StandardOutput.EndOfStream)
                {
                    sb.AppendLine(process.StandardOutput.ReadLine());
                }

                return sb.ToString();
            }
        }

        protected override string CallBatchImpl(byte[] batchPayload)
        {
            Console.WriteLine("**** OPERATION NOT SUPPORTED ****");
            return string.Empty;
        }
    }
}
