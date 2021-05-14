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

        public RegularResult deliverItems(String address, ConcurrentDictionary<int, int> items)
        {
            return new Ok("Deliver All Items Successfully!");
        }
    }
}
