using System;
using System.Collections.Concurrent;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TEST.UnitTests.UnitTestMocks

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