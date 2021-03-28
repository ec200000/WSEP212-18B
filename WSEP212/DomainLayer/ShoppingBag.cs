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

        public ShoppingBag(Store store)
        {
            this.store = store;
            this.items = new ConcurrentBag<Item>();
        }

        public bool addItem(int itemID);
        public bool removeItem(int itemID);
    }
}
