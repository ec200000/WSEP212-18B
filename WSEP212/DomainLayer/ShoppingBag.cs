using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class ShoppingBag
    {
        public Store store { get; set; }
        public LinkedList<Item> items { get; set; }

        public bool addItem(Item item);
        public bool removeItem(Item item);
    }
}
