using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StratisRpc
{
    public class ApiService
    {
        protected readonly string apiUrl;
        protected readonly string walletPassword;
        protected readonly short timeoutInSeconds;
        private HttpClient httpClient;
        private Stopwatch stopwatch;

        public ApiService(string apiUrl, string walletPassword, short timeoutInSeconds = 60)
        {
            this.apiUrl = apiUrl;
            this.walletPassword = walletPassword;
            this.timeoutInSeconds = timeoutInSeconds;

            this.stopwatch = new Stopwatch();
            this.httpClient = this.CreateHttpClient();
        }

        protected virtual HttpClient CreateHttpClient()
        {
            HttpClientHandler hch = new HttpClientHandler();
            hch.Proxy = null;
            hch.UseProxy = false;
            return new HttpClient(hch);
        }

        public void CallApi(bool showResult)
        {
            stopwatch.Restart();
            var result = this.httpClient.GetStringAsync($"{this.apiUrl}/Node/getrawtransaction?trxid={TestData.GetTxId()}&verbose=false").Result;
            stopwatch.Stop();

            if (showResult)
                Console.WriteLine($"CallApi [OK]: {result}");

            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds} ms\n\n");
        }

        public void CallMultipleApiAsync(bool showResult, int count)
        {
            var calls = new List<Task<string>>();
            stopwatch.Restart();
            for (int i = 0; i < count; i++)
            {
                var txId = TestData.GetTxId(i);
                calls.Add(this.httpClient.GetStringAsync($"{this.apiUrl}/Node/getrawtransaction?trxid={txId}&verbose=false"));
            }
            Task.WaitAll(calls.ToArray());
            stopwatch.Stop();

            if (showResult)
                Console.WriteLine($"CallMultipleApi [OK]:\n{string.Join('\n', calls.Select(c => c.Result)) }");

            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds} ms\n\n");
        }
    }
}
