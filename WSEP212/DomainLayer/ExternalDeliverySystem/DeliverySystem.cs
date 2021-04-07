using System;
using System.Collections.Concurrent;

namespace WSEP212.DomainLayer
{
    class DeliverySystem : DeliveryInterface
    {
        //singelton
        private static readonly Lazy<DeliverySystem> lazy
       = new Lazy<DeliverySystem>(() => new DeliverySystem());

        public static DeliverySystem Instance
            => lazy.Value;

        public bool deliverItems(String address, ConcurrentDictionary<int, int> items)
        {
            // deliver the items to the address
            return true;
        }
    }
}
