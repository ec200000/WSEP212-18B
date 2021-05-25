using System;
using WSEP212.DomainLayer.ExternalDeliverySystem;

namespace WSEP212.DomainLayer
{
    public class DeliverySystemMock : DeliveryInterface
    {
        //singelton
        private static readonly Lazy<DeliverySystemMock> lazy
            = new Lazy<DeliverySystemMock>(() => new DeliverySystemMock());

        public static DeliverySystemMock Instance
            => lazy.Value;

        private DeliverySystemMock() { }

        // returns valid transaction id
        public int deliverItems(string sendToName, string address, string city, string country, string zip)
        {
            return 10000;
        }

        public bool cancelDelivery(int transactionID)
        {
            return true;
        }
    }
}
