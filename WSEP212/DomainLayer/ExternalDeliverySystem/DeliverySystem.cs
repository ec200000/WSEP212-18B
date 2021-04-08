using System;
using System.Collections.Concurrent;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    class DeliverySystem : DeliveryInterface
    {
        //singelton
        private static readonly Lazy<DeliverySystem> lazy
       = new Lazy<DeliverySystem>(() => new DeliverySystem());

        public static DeliverySystem Instance
            => lazy.Value;

        public Result<Object> deliverItems(String address, ConcurrentDictionary<int, int> items)
        {
            // deliver the items to the address
            return new Ok<Object>("The Delivery Has Been Done Successfully", null);
        }
    }
}
