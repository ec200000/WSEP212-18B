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

        public RegularResult deliverItems(String address, ConcurrentDictionary<int, int> items)
        {
            return new Failure("Can Not Deliver Zero Items");
        }
    }
}