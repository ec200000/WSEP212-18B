using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    public interface DeliveryInterface
    {
        public RegularResult deliverItems(String address, ConcurrentDictionary<int, int> items);
    }
}
