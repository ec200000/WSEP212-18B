using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

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

        public Store(String storeName, SalesPolicy salesPolicy, PurchasePolicy purchasePolicy, User storeFounder)
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
            ConcurrentLinkedList<Permissions> founderPermissions = new ConcurrentLinkedList<Permissions>();
            founderPermissions.TryAdd(Permissions.AllPermissions);   // founder has all permisiions

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
        // if the item has the same name, category and price we treat the product as an existing product
        // the quantity of an existing product will update
        // returns the itemID of the item, otherwise -1
        public int addItemToStorage(int quantity, String itemName, String description, double price, String category)
        {
            if(quantity <= 0 || price <= 0 || itemName.Equals("") || category.Equals(""))
            {
                return -1;
            }
            // checks that the item not already exist
            foreach (KeyValuePair<int,Item> pairItem in storage)
            {
                Item item = pairItem.Value;
                if(item.itemName.Equals(itemName) && item.category.Equals(category) && item.price == price)   // the item already exist
                {
                    item.quantity += quantity;
                    return pairItem.Key;
                }
            }
            // item not exist - add new item to storage
            Item newItem = new Item(quantity, itemName, description, price, category);
            storage.TryAdd(newItem.itemID, newItem);
            return newItem.itemID;
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

        // returns the obj Item that corresponds to the requested ID number
        public Item getItemById(int itemID)
        {
            if(storage.ContainsKey(itemID))
            {
                return storage[itemID];
            }
            return null;
        }

        // edit the personal details of an item
        public bool editItem(int itemID, String itemName, String description, double price, String category, int quantity)
        {
            if (itemName.Equals("") || price <= 0 || category.Equals("") || quantity < 0)
                return false;
            // checks that the item exists
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                item.itemName = itemName;
                item.description = description;
                item.price = price;
                item.category = category;
                item.quantity = quantity;
                return true;
            }
            return false;
        }

        // purchase the items if the purchase request suitable with the store's policy and the products are also in stock
        public double purchaseItems(User buyer, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            double totalPrice = 0;
            if(applyPurchasePolicy(buyer, items, itemsPurchaseType))
            {
                if(purchaseItemsIfAvailable(items))
                {
                    ConcurrentDictionary<int, double> pricesAfterSale = applySalesPolicy(buyer, items);
                    foreach (KeyValuePair<int, double> itemPrice in pricesAfterSale)
                    {
                        int itemQuantity = items[itemPrice.Key];
                        totalPrice += itemPrice.Value * itemQuantity;
                    }
                    return totalPrice;
                }
            }
            return -1;
        }

        // Apply the sales policy on a list of items
        // The function will return the price after discount for each of the items
        // <itemID, price>
        public ConcurrentDictionary<int, double> applySalesPolicy(User buyer, ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<Item, int> objItems = getObjItems(items);
            if (objItems != null)
            {
                return salesPolicy.pricesAfterSalePolicy(buyer, objItems);
            }
            return null;
        }

        // Apply the purchase policy on a list of items and their type of purchase
        // The function will return if the purchase can be made
        public bool applyPurchasePolicy(User buyer, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            ConcurrentDictionary<Item, int> objItems = getObjItems(items);
            if(objItems != null)
            {
                return purchasePolicy.approveByPurchasePolicy(buyer, objItems, itemsPurchaseType);
            }
            return false;
        }

        private ConcurrentDictionary<Item, int> getObjItems(ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<Item, int> objItems = new ConcurrentDictionary<Item, int>();
            foreach (KeyValuePair<int, int> item in items)
            {
                int itemID = item.Key;
                if (!storage.ContainsKey(itemID))
                {
                    return null;
                }
                Item objItem = storage[itemID];
                objItems.TryAdd(objItem, item.Value);
            }
            return objItems;
        }

        // Purchase items from a store if all items are available in storage
        // The purchase of the items updates the quantity of the items in storage
        public bool purchaseItemsIfAvailable(ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<int, int> updatedItems = new ConcurrentDictionary<int, int>();

            foreach (KeyValuePair<int, int> item in items)
            {
                int itemID = item.Key;
                int quantity = item.Value;
                if (isAvailableInStorage(itemID, quantity))   // maybe lock the storage now
                {
                    changeItemQuantity(itemID, -1 * quantity);
                    updatedItems.TryAdd(itemID, quantity);
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
        public void rollBackPurchase(ConcurrentDictionary<int, int> items)
        {
            foreach (KeyValuePair<int, int> item in items)
            {
                changeItemQuantity(item.Key, item.Value);
            }
        }

        // Deliver all the items to the address
        // returns true if the delivery done successfully
        public bool deliverItems(String address, ConcurrentDictionary<int, int> items)
        {
            return DeliverySystem.Instance.deliverItems(address, items);
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
        public ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> getStoreOfficialsInfo()
        {
            ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> officialsInfo = new ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>>();
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
