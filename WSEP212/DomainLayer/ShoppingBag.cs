using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class ShoppingBag
    {
        public Store store { get; set; }
        public ConcurrentBag<Item> items { get; set; }

        public bool addItem(Item item);
        public bool removeItem(Item item);
    }
}
