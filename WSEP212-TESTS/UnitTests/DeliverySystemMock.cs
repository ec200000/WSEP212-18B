using System;
using System.Collections.Concurrent;
using WSEP212.DomainLayer;
using WSEP212.ServiceLayer.Result;

namespace WSEP212_TESTS
{
    public class DeliverySystemMock: DeliveryInterface
    {
        //singelton
        private static readonly Lazy<DeliverySystem> lazy
            = new Lazy<DeliverySystem>(() => new DeliverySystem());

        public static DeliverySystem Instance
            => lazy.Value;

        public RegularResult deliverItems(String address, ConcurrentDictionary<int, int> items)
        {
            return new Failure("Can Not Deliver Zero Items");
        }
    }
}