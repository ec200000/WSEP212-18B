﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace WSEP212.DomainLayer.ExternalPaymentSystem
{
    public class PaymentSystemAPI
    {
        //singelton
        private static readonly Lazy<PaymentSystemAPI> lazy
       = new Lazy<PaymentSystemAPI>(() => new PaymentSystemAPI());

        public static PaymentSystemAPI Instance
            => lazy.Value;

        private static readonly HttpClient client = new HttpClient();
        public static string webAddressAPI { get; set; }

        private PaymentSystemAPI() { }

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

        public async Task<int> paymentChargeAsync(string cardNumber, string month, string year, string holder, string ccv, string id, double price)
        {
            bool availability = await HttpHandshakeAsync();
            // checks that the external systems are available
            if(availability)
            {
                Dictionary<string, string> postValues = new Dictionary<string, string>
                {
                     { "action_type", "pay" },
                     { "card_number", cardNumber },
                     { "month", month },
                     { "year", year },
                     { "holder", holder },
                     { "ccv", ccv },
                     { "id", id }
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

        public async Task<bool> cancelPaymentChargeAsync(int transactionID)
        {
            bool availability = await HttpHandshakeAsync();
            // checks that the external systems are available
            if (availability)
            {
                Dictionary<string, string> postValues = new Dictionary<string, string>
                {
                     { "action_type", "cancel_pay" },
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
