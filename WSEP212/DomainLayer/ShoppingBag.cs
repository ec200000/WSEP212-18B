using System.Collections.Generic;

namespace WSEP212.DomainLayer
{
    class ShoppingBag
    {
        public Store store { get; set; }
        // There is no need for a structure that allows threads use, since only a single user can use these actions on his shopping bag
        public LinkedList<Item> items { get; set; }

        public ShoppingBag(Store store)
        {
            this.store = store;
            this.items = new LinkedList<Item>();
        }

        // Adds the item to the shopping bag if the item exist and available in the store
        public bool addItem(int itemID)
        {
            Item item = store.getItemIfAvailable(itemID);
            if(item != null)
            {
                items.AddFirst(item);
                return true;
            }
            return false;
        }

        // Removes the item from the shopping bag if it exists
        public bool removeItem(int itemID)
        {
            foreach (Item item in items)
            {
                if(itemID == item.itemID)
                {
                    items.Remove(item);
                    return true;
                }
            }
            return false;
        }
    }
}
