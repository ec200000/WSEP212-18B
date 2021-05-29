using System;
using WSEP212.DomainLayer.ExternalDeliverySystem;

namespace WSEP212.DomainLayer

{
    public class BadDeliverySystemMock: DeliveryInterface
    {
        //singelton
        private static readonly Lazy<BadDeliverySystemMock> lazy
            = new Lazy<BadDeliverySystemMock>(() => new BadDeliverySystemMock());

        public static BadDeliverySystemMock Instance
            => lazy.Value;

        private BadDeliverySystemMock() { }

        // bad delivery system, always reject the delivery 
        public int deliverItems(string sendToName, string address, string city, string country, string zip)
        {
            return -1;
        }

        public bool cancelDelivery(int transactionID)
        {
            return false;
        }
    }
}