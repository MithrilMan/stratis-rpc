using System;

namespace bitcoin_rpc
{
    public static class IRpcServiceExtensions
    {
        public static void TestThis(this IRpcService rpcService, string message, Action testToExecute)
        {
            Console.WriteLine($@"
{"=".PadRight(20, '=')}{message} - Using {rpcService.GetServiceDescription()}
");

            testToExecute();
            Console.WriteLine("=".PadRight(80, '='));
        }
    }
}
