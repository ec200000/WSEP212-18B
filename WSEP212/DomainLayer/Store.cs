using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class Store
    {
        // static counter for the storeIDs of diffrent stores
        private static int storeCounter = 1;

        // A data structure associated with a item ID and its item
        public ConcurrentDictionary<int, Item> storage { get; set; }
        public int storeID { get; set; }
        public SalesPolicy salesPolicy { get; set; }
        public PurchasePolicy purchasePolicy { get; set; }
        public ConcurrentBag<PurchaseInfo> purchases { get; set; }
        public ConcurrentBag<SellerPermissions> sellersPermissions { get; set; }

        public Store(SalesPolicy salesPolicy, PurchasePolicy purchasePolicy)
        {
            this.storage = new ConcurrentDictionary<int, Item>();
            this.storeID = storeCounter;
            storeCounter++;
            this.salesPolicy = salesPolicy;
            this.purchasePolicy = purchasePolicy;
            this.purchases = new ConcurrentBag<PurchaseInfo>();
            // seller permissions ??
        }

        // Check if the item exist and available in the store
        public bool isAvailableInStorage(int itemID, int quantity)
        {
            if(storage.ContainsKey(itemID))   // checks if item exist in the store
            {
                Item item = storage[itemID];
                if (quantity <= item.quantity)   // checks if there is enough of the item in stock
                {
                    return true;
                }
            }
            return false;
        }

        // Add new item to the store with his personal details
        public bool addItemToStorage(String itemName, String description, double price, String category, int quantity)
        {
            Item item = new Item(quantity, itemName, description, price, category);
            return storage.TryAdd(item.itemID, item);
        }

        // Remove item from the store
        // If the item has n quantity in the store, all the n will be deleted
        public bool removeItemFromStorage(int itemID)
        {
            if(storage.ContainsKey(itemID))
            {
                return storage.TryRemove(itemID, out _);
            }
            return false;
        }

        // Change item quantity in the storage of the store
        // numOfItems can be positive or negative - which means remove or add to the storage
        public bool changeItemQuantity(int itemID, int numOfItems)
        {
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                item.quantity += numOfItems;
                return true;
            }
            return false;
        }

        // edit the personal details of an item
        public bool editItem(int itemID, String itemName, String description, double price, String category)
        {
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                item.itemName = itemName;
                item.description = description;
                item.price = price;
                item.category = category;
                return true;
            }
            return false;
        }

        // Apply the sales policy on a list of items
        // The function will return the price after discount for each of the items
        public LinkedList<int> applySalesPolicy(Dictionary<int, int> items)
        {
            
        }

        // Apply the purchase policy on a list of items and their type of purchase
        // The function will return if the purchase can be made
        public bool applyPurchasePolicy(Dictionary<int, int> items)
        {

        }

        // Purchase items from a store if all items are available in storage
        // The purchase of the items updates the quantity of the items in storage
        public bool purchaseItemsIfAvailable(Dictionary<int, int> items)
        {
            Dictionary<int, int> updatedItems = new Dictionary<int, int>();

            foreach (KeyValuePair<int, int> item in items)
            {
                int itemID = item.Key;
                int quantity = item.Value;
                if (isAvailableInStorage(itemID, quantity))   // maybe lock the storage now
                {
                    changeItemQuantity(itemID, -1 * quantity);
                    updatedItems.Add(itemID, quantity);
                }
                else
                {
                    rollBackPurchase(updatedItems);   // if at least one of the items not available, the purchase is canceled
                    return false;
                }
            }
            return true;
        }

        // In case the purchase is canceled (payment is not made / system collapses) -
        // the items that were supposed to be purchased return to the store
        public void rollBackPurchase(Dictionary<int, int> items)
        {
            foreach (KeyValuePair<int, int> item in items)
            {
                changeItemQuantity(item.Key, item.Value);
            }
        }

        public bool addNewStoreSeller(SellerPermissions sellerPermissions); //add to list
        public bool addNewPurchase(PurchaseInfo purchase); //add to list
    }
}
