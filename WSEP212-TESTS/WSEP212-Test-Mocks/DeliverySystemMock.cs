using System;
using System.Collections.Concurrent;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TEST.UnitTests.UnitTestMocks
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
