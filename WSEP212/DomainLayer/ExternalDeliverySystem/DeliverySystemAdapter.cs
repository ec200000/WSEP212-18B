using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEP212.DomainLayer.ExternalDeliverySystem
{
    public class DeliverySystemAdapter : DeliveryInterface
    {
        //singelton
        private static readonly Lazy<DeliverySystemAdapter> lazy
            = new Lazy<DeliverySystemAdapter>(() => new DeliverySystemAdapter());

        public static DeliverySystemAdapter Instance => lazy.Value;

        private DeliverySystemAPI deliverySystemAPI { get; set; }

        private DeliverySystemAdapter() 
        {
            this.deliverySystemAPI = DeliverySystemAPI.Instance;
        }

        public int deliverItems(string sendToName, string address, string city, string country, string zip)
        {
            return this.deliverySystemAPI.deliverItemsAsync(sendToName, address, city, country, zip).Result;
        }

        public bool cancelDelivery(int transactionID)
        {
            return this.deliverySystemAPI.cancelDeliveryAsync(transactionID).Result;
        }
    }
}
