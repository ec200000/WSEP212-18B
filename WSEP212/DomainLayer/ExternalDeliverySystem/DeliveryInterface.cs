using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public interface DeliveryInterface
    {
        public bool deliverItems(String address, ConcurrentDictionary<int, int> items);
    }
}
