using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StratisRpc.CallRequest
{
    public abstract class TestRequest
    {
        /// <summary>
        /// The method to test.
        /// </summary>
        public string MethodName { get; protected set; }

        /// <summary>
        /// The list of parameters to pass to the call.
        /// The insertion order matters.
        /// </summary>
        public readonly List<(string Name, object Value)> Parameters;

        public TestRequest(MethodToTest methodToTest)
        {
            this.MethodName = methodToTest.ToString();
            this.Parameters = new List<(string, object)>();
        }

        public TestRequest(string methodToTest)
        {
            this.MethodName = methodToTest;
            this.Parameters = new List<(string, object)>();
        }

        /// <summary>
        /// Adds the specified parameter value.
        /// If the passed value is null, it will not be included in the generated request.
        /// Fluent API.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public TestRequest AddParameter(string parameterName, object value)
        {
            //object usedValue = value;
            //if (value is bool val)
            //    usedValue = val ? "1" : "0";

            this.Parameters.Add((Name: parameterName.ToLower(), Value: value));
            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.MethodName);

            foreach (var parameter in this.Parameters)
            {
                sb.Append($" {parameter.Name}:{parameter.Value}");
            }

            return sb.ToString();
        }

        public TReturnType GetParameter<TReturnType>(int index)
        {
            return (TReturnType)this.Parameters[index].Value;
        }

        public TReturnType GetParameter<TReturnType>(string parameterName)
        {
            parameterName = parameterName.ToLower();
            return (TReturnType)this.Parameters.Find(p => p.Name == parameterName).Value;
        }

        /// <summary>
        /// Returns the Json formatted request.
        /// </summary>
        /// <returns></returns>
        public string ToJson(int RequestId)
        {
            var json = JsonConvert.SerializeObject(new
            {
                method = this.MethodName.ToLower(),
                @params = this.Parameters
                    .Where(p => p.Value != null)
                    .Select(p => p.Value),
                id = RequestId
            });

            return json;
        }

        /// <summary>
        /// Gets the bytes of the generated payload to be used for RPC calls.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes(int RequestId)
        {
            return Encoding.UTF8.GetBytes(ToJson(RequestId));
        }
    }
}