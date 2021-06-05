using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.ExternalDeliverySystem
{
    public class DeliverySystemAPI
    {
        //singelton
        private static readonly Lazy<DeliverySystemAPI> lazy
            = new Lazy<DeliverySystemAPI>(() => new DeliverySystemAPI());

        public static DeliverySystemAPI Instance
            => lazy.Value;

        private static readonly HttpClient client = new HttpClient();
        public static string webAddressAPI { get; set; }
        
        private DeliverySystemAPI() { }

        private async Task<bool> HttpHandshakeAsync()
        {
            Dictionary<string, string> postValues = new Dictionary<string, string>
            {
                { "action_type", "handshake" }
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(postValues);
            // sending the post request to the web address
            var response = await client.PostAsync(webAddressAPI, content);
            // read web response - expecting "OK"
            string responseString = await response.Content.ReadAsStringAsync();
            return responseString.Equals("OK");
        }

        public async Task<int> deliverItemsAsync(string sendToName, string address, string city, string country, string zip)
        {
            bool availability = await HttpHandshakeAsync();
            // checks that the external systems are available
            if (availability)
            {
                Dictionary<string, string> postValues = new Dictionary<string, string>
                {
                     { "action_type", "supply" },
                     { "name", sendToName },
                     { "address", address },
                     { "city", city },
                     { "country", country },
                     { "zip", zip }
                };
                FormUrlEncodedContent content = new FormUrlEncodedContent(postValues);
                // sending the post request to the web address
                var response = await client.PostAsync(webAddressAPI, content);
                // read web response - expecting integer
                string responseString = await response.Content.ReadAsStringAsync();
                return int.Parse(responseString);
            }
            return -1;
        }

        public async Task<bool> cancelDeliveryAsync(int transactionID)
        {
            bool availability = await HttpHandshakeAsync();
            // checks that the external systems are available
            if (availability)
            {
                Dictionary<string, string> postValues = new Dictionary<string, string>
                {
                     { "action_type", "cancel_supply" },
                     { "transaction_id", transactionID.ToString() }
                };
                FormUrlEncodedContent content = new FormUrlEncodedContent(postValues);
                // sending the post request to the web address
                var response = await client.PostAsync(webAddressAPI, content);
                // read web response - expecting integer
                string responseString = await response.Content.ReadAsStringAsync();
                return int.Parse(responseString) == 1;
            }
            return false;
        }
    }
}
