using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace StratisRpc.CallResponse
{
    [JsonConverter(typeof(ListAddressGroupingsResponseConverter))]
    public class ListAddressGroupingsResponse
    {
        public class AddressDetails
        {
            public string Address { get; set; }
            public decimal Balance { get; set; }
            public string Account { get; set; }
        }

        public List<List<AddressDetails>> AddressesGroups { get; set; }
    }


    public class ListAddressGroupingsResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ListAddressGroupingsResponse);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var addressesGroups = JArray.Load(reader);

            ListAddressGroupingsResponse result = new ListAddressGroupingsResponse();
            result.AddressesGroups = new List<List<ListAddressGroupingsResponse.AddressDetails>>(addressesGroups.Count);

            foreach (JArray addressesGroupToken in addressesGroups)
            {
                List<ListAddressGroupingsResponse.AddressDetails> addressesGroup = new List<ListAddressGroupingsResponse.AddressDetails>();

                foreach (JArray addressDetailToken in addressesGroupToken)
                {
                    var addressDetail = new ListAddressGroupingsResponse.AddressDetails();
                    addressDetail.Address = addressDetailToken[0].Value<string>();
                    addressDetail.Balance = addressDetailToken[1].Value<decimal>();

                    if (addressDetailToken.Count > 2)
                        addressDetail.Account = addressDetailToken[2].Value<string>();

                    addressesGroup.Add(addressDetail);
                }

                result.AddressesGroups.Add(addressesGroup);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
