using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class ShoppingCart
    {
        public ConcurrentBag<ShoppingBag> shoppingBags { get; set; }

        public bool addItemToShoppingBag(int storeID, int itemID);
        public bool removeItemFromShoppingBag(int storeID, int itemID);
    }
}
