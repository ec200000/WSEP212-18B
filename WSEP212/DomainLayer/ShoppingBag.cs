using System.Collections.Generic;

namespace WSEP212.DomainLayer
{
    class ShoppingBag
    {
        public Store store { get; set; }
        // A data structure associated with a item ID and its quantity - more effective when there will be a sales policy
        // There is no need for a structure that allows threads use, since only a single user can use these actions on his shopping bag
        public Dictionary<int, int> items { get; set; }

        public ShoppingBag(Store store)
        {
            this.store = store;
            this.items = new Dictionary<int, int>();
        }

        // Adds one item to the shopping bag if the item exist and available in the store
        public bool addItem(int itemID)
        {
            if(store.isAvailableInStorage(itemID, 1))
            {
                if(items.ContainsKey(itemID))
                {
                    items[itemID] += 1;
                }
                else
                {
                    items.Add(itemID, 1);   // adding item with quantity 1
                }
                return true;
            }
            return false;
        }

        // Removes the item from the shopping bag if it exists
        // If the item has n quantity in the basket, all the n will be deleted
        public bool removeItem(int itemID)
        {
            if(items.ContainsKey(itemID))
            {
                items.Remove(itemID);
                return true;
            }
            return false;
        }

        // Changes the quantity of item in the bag if the item is in the bag and available in the store
        public bool changeItemQuantity(int itemID, int quantity)
        {
            if(quantity == 0)
            {
                return removeItem(itemID);
            }
            else if(items.ContainsKey(itemID))  // check if item in the shopping bag
            {
                if(store.isAvailableInStorage(itemID, quantity))   // check if item available in store
                {
                    items[itemID] = quantity;
                    return true;
                }
            }
            return false;
        }

    }
}
