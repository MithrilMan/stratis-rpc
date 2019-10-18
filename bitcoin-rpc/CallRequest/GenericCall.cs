namespace StratisRpc.CallRequest
{
    /// <summary>
    /// Generic RPC call
    /// </summary>
    public class GenericCall : TestRequest
    {
        public string Address => this.GetParameter<string>(0);

        public GenericCall(string methodName, params (string argument, object value)[] arguments) : base(methodName)
        {
            foreach (var argument in arguments)
            {
                this.AddParameter(argument.argument, argument.value);
            }
        }
    }
}
