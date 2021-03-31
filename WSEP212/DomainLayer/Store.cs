using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class Store
    {
        // static counter for the storeIDs of diffrent stores
        private static int storeCounter = 1;

        // A data structure associated with a item ID and its item
        public ConcurrentDictionary<int, Item> storage { get; set; }
        public int storeID { get; set; }

        public string storeName { get; set; }
        public bool activeStore { get; set; }
        public SalesPolicy salesPolicy { get; set; }
        public PurchasePolicy purchasePolicy { get; set; }
        public ConcurrentBag<PurchaseInfo> purchasesHistory { get; set; }
        // A data structure associated with a user name and seller permissions
        public ConcurrentDictionary<String, SellerPermissions> storeSellersPermissions { get; set; }

        public Store(SalesPolicy salesPolicy, PurchasePolicy purchasePolicy, User storeFounder, string storeName)
        {
            this.storage = new ConcurrentDictionary<int, Item>();
            this.storeID = storeCounter;
            storeCounter++;
            this.activeStore = true;
            this.salesPolicy = salesPolicy;
            this.purchasePolicy = purchasePolicy;
            this.purchasesHistory = new ConcurrentBag<PurchaseInfo>();
            this.storeName = storeName;

            // create the founder seller permissions
            LinkedList<Permissions> founderPermissions = new LinkedList<Permissions>();
            founderPermissions.AddFirst(Permissions.AllPermissions);   // founder has all permisiions

            SellerPermissions storeFounderPermissions = SellerPermissions.getSellerPermissions(storeFounder, this, null, founderPermissions);

            this.storeSellersPermissions = new ConcurrentDictionary<String, SellerPermissions>();
            this.storeSellersPermissions.TryAdd(storeFounder.userName, storeFounderPermissions);
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
        public bool addItemToStorage(Item item)
        {
            if (item.itemName == "" || item.price <= 0 || item.category == "" || item.quantity < 0)
                return false;
            int itemID = item.itemID;
            if(!storage.ContainsKey(itemID))
            {
                return storage.TryAdd(itemID, item);
            }
            return false;
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

        public Item getItemById(int itemID)
        {
            if(storage.ContainsKey(itemID))
            {
                return storage[itemID];
            }
            return null;
        }

        // edit the personal details of an item
        public bool editItem(int itemID, String itemName, String description, double price, String category)
        {
            if (itemName == "" || price < 0 || category == "")
                return false;
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
            return null;
        }

        // Apply the purchase policy on a list of items and their type of purchase
        // The function will return if the purchase can be made
        public bool applyPurchasePolicy(Dictionary<int, int> items)
        {
            return false;
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

        // Adds a new store seller for this store
        public bool addNewStoreSeller(SellerPermissions sellerPermissions)
        {
            String sellerUserName = sellerPermissions.seller.userName;
            if (!storeSellersPermissions.ContainsKey(sellerUserName))
            {
                return storeSellersPermissions.TryAdd(sellerUserName, sellerPermissions);
            }
            return false;
        }

        // Removes a store seller from this store
        public bool removeStoreSeller(String sellerUserName)
        {
            if (storeSellersPermissions.ContainsKey(sellerUserName))
            {
                return storeSellersPermissions.TryRemove(sellerUserName, out _);
            }
            return false;
        }

        // Returns information about all the store official (store owner and manager) in the store
        public ConcurrentDictionary<User, ConcurrentBag<Permissions>> getStoreOfficialsInfo()
        {
            ConcurrentDictionary<User, ConcurrentBag<Permissions>> officialsInfo = new ConcurrentDictionary<User, ConcurrentBag<Permissions>>();
            foreach (KeyValuePair<string, SellerPermissions> sellerEntry in storeSellersPermissions)
            {
                SellerPermissions seller = sellerEntry.Value;
                officialsInfo.TryAdd(seller.seller, seller.permissionsInStore);
            }
            return officialsInfo;
        }

        // Add purchase made by user in the store
        public void addNewPurchase(PurchaseInfo purchase)
        {
            purchasesHistory.Add(purchase);
        }
    }
}
