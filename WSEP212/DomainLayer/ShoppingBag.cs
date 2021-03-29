using System.Collections.Generic;

namespace WSEP212.DomainLayer
{
    public class ShoppingBag
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

        // return true if the shopping bag is empty
        public bool isEmpty()
        {
            return items.Count == 0;
        }

        // Adds item to the shopping bag if the item exist and available in the store
        // quantity is the number of the same item to add
        public bool addItem(int itemID, int quantity)
        {
            if(store.isAvailableInStorage(itemID, quantity))
            {
                if(items.ContainsKey(itemID))
                {
                    items[itemID] += quantity;
                }
                else
                {
                    items.Add(itemID, quantity);   // adding item with quantity 
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
