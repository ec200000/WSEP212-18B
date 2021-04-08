using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    public class Store
    {
        // static counter for the storeIDs of diffrent stores
        private static int storeCounter = 1;

        // A data structure associated with a item ID and its item
        public ConcurrentDictionary<int, Item> storage { get; set; }
        public int storeID { get; set; }
        public String storeName { get; set; }
        public String storeAddress { get; set; }
        public bool activeStore { get; set; }
        public SalesPolicy salesPolicy { get; set; }
        public PurchasePolicy purchasePolicy { get; set; }
        public ConcurrentBag<PurchaseInfo> purchasesHistory { get; set; }
        // A data structure associated with a user name and seller permissions
        public ConcurrentDictionary<String, SellerPermissions> storeSellersPermissions { get; set; }

        public Store(String storeName, String storeAddress, SalesPolicy salesPolicy, PurchasePolicy purchasePolicy, User storeFounder)
        {
            this.storage = new ConcurrentDictionary<int, Item>();
            this.storeID = storeCounter;
            storeCounter++;
            this.activeStore = true;
            this.salesPolicy = salesPolicy;
            this.purchasePolicy = purchasePolicy;
            this.purchasesHistory = new ConcurrentBag<PurchaseInfo>();
            this.storeName = storeName;
            this.storeAddress = storeAddress;

            // create the founder seller permissions
            ConcurrentLinkedList<Permissions> founderPermissions = new ConcurrentLinkedList<Permissions>();
            founderPermissions.TryAdd(Permissions.AllPermissions);   // founder has all permisiions

            SellerPermissions storeFounderPermissions = SellerPermissions.getSellerPermissions(storeFounder, this, null, founderPermissions);

            this.storeSellersPermissions = new ConcurrentDictionary<String, SellerPermissions>();
            this.storeSellersPermissions.TryAdd(storeFounder.userName, storeFounderPermissions);
        }

        // Check if the item exist and available in the store
        public Result<Object> isAvailableInStorage(int itemID, int quantity)
        {
            if(storage.ContainsKey(itemID))   // checks if item exist in the store
            {
                Item item = storage[itemID];
                if (quantity <= item.quantity)   // checks if there is enough of the item in stock
                {
                    return new Ok<Object>("The Item Is Available In The Store Storage", null);
                }
                return new Failure<Object>("The Item Is Not Available In The Store Storage At The Moment", null);
            }
            return new Failure<Object>("Item Not Exist In Storage", null);
        }

        // Add new item to the store with his personal details
        // if the item has the same name, category and price we treat the product as an existing product
        // the quantity of an existing product will update
        // returns the itemID of the item, otherwise -1
        public Result<int> addItemToStorage(int quantity, String itemName, String description, double price, String category)
        {
            if(quantity <= 0 || price <= 0 || itemName.Equals("") || category.Equals(""))
            {
                return new Failure<int>("One Or More Of The Item Details Are Invalid", -1);
            }
            // checks that the item not already exist
            foreach (KeyValuePair<int,Item> pairItem in storage)
            {
                Item item = pairItem.Value;
                if(item.itemName.Equals(itemName) && item.category.Equals(category) && item.price == price)   // the item already exist
                {
                    item.quantity += quantity;
                    return new Ok<int>("The Item Is Already In Storage, The Quantity Of The Item Has Been Updated Accordingly", pairItem.Key);
                }
            }
            // item not exist - add new item to storage
            Item newItem = new Item(quantity, itemName, description, price, category);
            storage.TryAdd(newItem.itemID, newItem);
            return new Ok<int>("The Item Was Successfully Added To The Store Storage", newItem.itemID);
        }

        // Remove item from the store
        // If the item has n quantity in the store, all the n will be deleted
        public Result<Object> removeItemFromStorage(int itemID)
        {
            if(storage.ContainsKey(itemID))
            {
                storage.TryRemove(itemID, out _);
                return new Ok<Object>("Item Was Successfully Removed From The Store's Storage", null);
            }
            return new Failure<Object>("Item Not Exist In Storage", null);
        }

        // Change item quantity in the storage of the store
        // numOfItems can be positive or negative - which means remove or add to the storage
        public Result<Object> changeItemQuantity(int itemID, int numOfItems)
        {
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                item.quantity += numOfItems;
                return new Ok<Object>("The Item Quantity In The Store's Storage Has Been Successfully Changed", null);
            }
            return new Failure<Object>("Item Is Not Exist In Storage", null);
        }

        // returns the obj Item that corresponds to the requested ID number
        public Result<Item> getItemById(int itemID)
        {
            if(storage.ContainsKey(itemID))
            {
                return new Ok<Item>("Item Found In Storage Successfully", storage[itemID]);
            }
            return new Failure<Item>("Item Not Exist In Storage", null);
        }

        // edit the personal details of an item
        public Result<Object> editItem(int itemID, String itemName, String description, double price, String category, int quantity)
        {
            if (itemName.Equals("") || price <= 0 || category.Equals("") || quantity < 0)
                return new Failure<Object>("One Or More Of The New Item Details Are Invalid", null);
            // checks that the item exists
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                item.itemName = itemName;
                item.description = description;
                item.price = price;
                item.category = category;
                item.quantity = quantity;
                return new Ok<Object>("Item Details Have Been Successfully Updated In The Store", null);
            }
            return new Failure<Object>("Item Not Exist In Storage", null);
        }

        // purchase the items if the purchase request suitable with the store's policy and the products are also in stock
        public Result<double> purchaseItems(User buyer, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            double totalPrice = 0;

            // checks the purchase can be made by the purchase policy
            Result<Object> purchasePolicyRes = applyPurchasePolicy(buyer, items, itemsPurchaseType);
            if (purchasePolicyRes.getTag())
            {
                // checks that all the items available in the store storage
                Result<Object> availableItemsRes = purchaseItemsIfAvailable(items);
                if (availableItemsRes.getTag())
                {
                    // calculate the price of each item after sales
                    Result<ConcurrentDictionary<int, double>> pricesAfterSaleRes = applySalesPolicy(buyer, items);
                    if(pricesAfterSaleRes.getTag())
                    {
                        foreach (KeyValuePair<int, double> itemPrice in pricesAfterSaleRes.getValue())
                        {
                            totalPrice += itemPrice.Value * items[itemPrice.Key];   // item price multiple by his quantity in the purchase
                        }
                        return new Ok<double>("The Purchase Can Be Made, The Items Are Available In Storage And The Final Price Calculated For Each Item", totalPrice);
                    }
                    return new Failure<double>(pricesAfterSaleRes.getMessage(), -1);
                }
                return new Failure<double>(availableItemsRes.getMessage(), -1);
            }
            return new Failure<double>(purchasePolicyRes.getMessage(), -1);
        }

        // Apply the sales policy on a list of items
        // The function will return the price after discount for each of the items
        // <itemID, price>
        public Result<ConcurrentDictionary<int, double>> applySalesPolicy(User buyer, ConcurrentDictionary<int, int> items)
        {
            Result<ConcurrentDictionary<Item, int>> objItemsRes = getObjItems(items);
            if (objItemsRes.getTag())
            {
                ConcurrentDictionary<int, double> priceAfterSales = salesPolicy.pricesAfterSalePolicy(buyer, objItemsRes.getValue());
                return new Ok<ConcurrentDictionary<int, double>>("The Sales On The Items Have Been Made Successfully", priceAfterSales);
            }
            return new Failure<ConcurrentDictionary<int, double>>(objItemsRes.getMessage(), null);
        }

        // Apply the purchase policy on a list of items and their type of purchase
        // The function will return if the purchase can be made
        public Result<Object> applyPurchasePolicy(User buyer, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            Result<ConcurrentDictionary<Item, int>> objItemsRes = getObjItems(items);
            if(objItemsRes.getTag())
            {
                return purchasePolicy.approveByPurchasePolicy(buyer, objItemsRes.getValue(), itemsPurchaseType);
            }
            return new Failure<Object>(objItemsRes.getMessage(), null);
        }

        private Result<ConcurrentDictionary<Item, int>> getObjItems(ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<Item, int> objItems = new ConcurrentDictionary<Item, int>();
            foreach (KeyValuePair<int, int> item in items)
            {
                int itemID = item.Key;
                if (!storage.ContainsKey(itemID))
                {
                    return new Failure<ConcurrentDictionary<Item, int>>("One Or More Of The Items Not Exist In Storage", null);
                }
                Item objItem = storage[itemID];
                objItems.TryAdd(objItem, item.Value);
            }
            return new Ok<ConcurrentDictionary<Item, int>>("All Items Exist In Storage", objItems);
        }

        // Purchase items from a store if all items are available in storage
        // The purchase of the items updates the quantity of the items in storage
        public Result<Object> purchaseItemsIfAvailable(ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<int, int> updatedItems = new ConcurrentDictionary<int, int>();

            foreach (KeyValuePair<int, int> item in items)
            {
                int itemID = item.Key;
                int quantity = item.Value;
                Result<Object> itemAvailableRes = isAvailableInStorage(itemID, quantity);
                if (itemAvailableRes.getTag())   // maybe lock the storage now
                {
                    Result<Object> changeQuantityRes = changeItemQuantity(itemID, -1 * quantity);
                    if(changeQuantityRes.getTag())
                    {
                        updatedItems.TryAdd(itemID, quantity);
                    }
                    else
                    {
                        return new Failure<Object>("One Or More Of The " + changeQuantityRes.getMessage(), null);
                    }
                }
                else
                {
                    rollBackPurchase(updatedItems);   // if at least one of the items not available, the purchase is canceled
                    return new Failure<Object>("One Or More Of " + itemAvailableRes.getMessage(), null);
                }
            }
            return new Ok<Object>("All Items Are Available In The Store's Storage", null);
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
        public Result<Object> deliverItems(String address, ConcurrentDictionary<int, int> items)
        {
            return DeliverySystem.Instance.deliverItems(address, items);
        }

        // Adds a new store seller for this store
        public Result<Object> addNewStoreSeller(SellerPermissions sellerPermissions)
        {
            String sellerUserName = sellerPermissions.seller.userName;
            if (!storeSellersPermissions.ContainsKey(sellerUserName))
            {
                storeSellersPermissions.TryAdd(sellerUserName, sellerPermissions);
                return new Ok<Object>("The New Store Seller Added To The Store Successfully", null);
            }
            return new Failure<Object>("The Store Seller Is Already Defined As A Seller In This Store", null);
        }

        // Removes a store seller from this store
        public Result<Object> removeStoreSeller(String sellerUserName)
        {
            if (storeSellersPermissions.ContainsKey(sellerUserName))
            {
                storeSellersPermissions.TryRemove(sellerUserName, out _);
                return new Ok<Object>("The Store Seller Removed From The Store Successfully", null);
            }
            return new Failure<Object>("The Store Seller Is Not Defined As A Seller In This Store", null);
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

        public static void resetStoreCounter()
        {
            Store.storeCounter = 1;
        }
    }
}
