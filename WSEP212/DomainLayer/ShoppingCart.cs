using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class ShoppingCart
    {
        public LinkedList<ShoppingBag> shoppingBags { get; set; }

        public bool addItemToShoppingBag(int storeID, int itemID);
        public bool removeItemFromShoppingBag(int storeID, int itemID);
    }
}
