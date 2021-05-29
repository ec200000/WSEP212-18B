using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WSEP212.DomainLayer
{
    public class LocalPredicateSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var name = value as LocalPredicate<PurchaseDetails>;
            writer.WriteStartObject();
            writer.WritePropertyName("$" + name.biggerThan);
            serializer.Serialize(writer, name.method.ToString());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();
            Expression<Func<PurchaseDetails, double>> exp = null;
            if(((string)properties[0].Value).Contains("userAge"))
            {
                exp = pd => pd.userAge();
            }
            else if(((string)properties[0].Value).Contains("totalPurchasePrice"))
            {
                exp = pd => pd.totalPurchasePrice();
            }
            else
            {
                exp = pd => pd.numOfItemsInPurchase();
            }
            return new LocalPredicate<PurchaseDetails>() {
                biggerThan = int.Parse(properties[0].Name.Replace("$","")),
                method = exp
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(LocalPredicate<PurchaseDetails>).IsAssignableFrom(objectType);
        }
    }
}