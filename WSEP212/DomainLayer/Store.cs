﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    [Serializable]
    public class Store
    {
        private readonly object itemInStorageLock = new object();
        private readonly object purchaseItemsLock = new object();
        private readonly object addStoreSellerLock = new object();

        // static counter for the storeIDs of diffrent stores
        private static int storeCounter = 1;

        // A data structure associated with a item ID and its item
        public ConcurrentDictionary<int, Item> storage { get; set; }
        public int storeID { get; set; }
        public String storeName { get; set; }
        public String storeAddress { get; set; }
        public bool activeStore { get; set; }
        public SalePolicyInterface salesPolicy { get; set; }
        public PurchasePolicyInterface purchasePolicy { get; set; }
        public ConcurrentBag<PurchaseInvoice> purchasesHistory { get; set; }
        // A data structure associated with a user name and seller permissions
        public ConcurrentDictionary<String, SellerPermissions> storeSellersPermissions { get; set; }
        public DeliveryInterface deliverySystem { get; set; }

        public Store(String storeName, String storeAddress, SalePolicyInterface salesPolicy, PurchasePolicyInterface purchasePolicy, User storeFounder)
        {
            this.storage = new ConcurrentDictionary<int, Item>();
            this.storeID = storeCounter;
            storeCounter++;
            this.activeStore = true;
            this.salesPolicy = salesPolicy;
            this.purchasePolicy = purchasePolicy;
            this.purchasesHistory = new ConcurrentBag<PurchaseInvoice>();
            this.storeName = storeName;
            this.storeAddress = storeAddress;

            // create the founder seller permissions
            ConcurrentLinkedList<Permissions> founderPermissions = new ConcurrentLinkedList<Permissions>();
            founderPermissions.TryAdd(Permissions.AllPermissions);   // founder has all permisiions

            SellerPermissions storeFounderPermissions = SellerPermissions.getSellerPermissions(storeFounder, this, null, founderPermissions);

            this.storeSellersPermissions = new ConcurrentDictionary<String, SellerPermissions>();
            this.storeSellersPermissions.TryAdd(storeFounder.userName, storeFounderPermissions);

            this.deliverySystem = DeliverySystem.Instance;
        }

        // Check if the item exist and available in the store
        public RegularResult isAvailableInStorage(int itemID, int quantity)
        {
            lock (itemInStorageLock)
            {
                if (storage.ContainsKey(itemID))   // checks if item exist in the store
                {
                    Item item = storage[itemID];
                    if (quantity <= item.quantity)   // checks if there is enough of the item in stock
                    {
                        return new Ok("The Item Is Available In The Store Storage");
                    }
                    return new Failure("The Item Is Not Available In The Store Storage At The Moment");
                }
                return new Failure("Item Not Exist In Storage");
            }
        }

        // Add new item to the store with his personal details
        // if the item has the same name, category and price we treat the product as an existing product
        // the quantity of an existing product will update
        // returns the itemID of the item, otherwise -1
        public ResultWithValue<int> addItemToStorage(int quantity, String itemName, String description, double price, String category)
        {
            if (price <= 0 || itemName.Equals("") || category.Equals("") || quantity < 0)
            {
                return new FailureWithValue<int>("One Or More Of The Item Details Are Invalid", -1);
            }
            // checks that the item not already exist
            foreach (KeyValuePair<int, Item> pairItem in storage)
            {
                Item item = pairItem.Value;
                if (item.itemName.Equals(itemName) && item.category.Equals(category) && item.price == price)   // the item already exist
                {
                    if (item.setQuantity(quantity))
                        return new OkWithValue<int>("The Item Is Already In Storage, The Quantity Of The Item Has Been Updated Accordingly", pairItem.Key);
                    return new FailureWithValue<int>("One Or More Of The Item Details Are Invalid", -1);
                }
            }
            // item not exist - add new item to storage
            Item newItem = new Item(quantity, itemName, description, price, category);
            this.storage.TryAdd(newItem.itemID, newItem);
            return new OkWithValue<int>("The Item Was Successfully Added To The Store Storage", newItem.itemID);
        }

        // Remove item from the store
        // If the item has n quantity in the store, all the n will be deleted
        public RegularResult removeItemFromStorage(int itemID)
        {
            if (storage.ContainsKey(itemID))
            {
                storage.TryRemove(itemID, out _);
                return new Ok("Item Was Successfully Removed From The Store's Storage");
            }
            return new Failure("Item Not Exist In Storage");
        }

        // Change item quantity in the storage of the store
        // numOfItems can be positive or negative - which means remove or add to the storage
        public RegularResult changeItemQuantity(int itemID, int numOfItems)
        {
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                if (item.changeQuantity(numOfItems))
                    return new Ok("The Item Quantity In The Store's Storage Has Been Successfully Changed");
                return new Failure("Item quantity can't be negative");

            }
            return new Failure("Item Is Not Exist In Storage");
        }

        // returns the obj Item that corresponds to the requested ID number
        public ResultWithValue<Item> getItemById(int itemID)
        {
            if (storage.ContainsKey(itemID))
            {
                return new OkWithValue<Item>("Item Found In Storage Successfully", storage[itemID]);
            }
            return new FailureWithValue<Item>("Item Not Exist In Storage", null);
        }

        // edit the personal details of an item
        public RegularResult editItem(int itemID, String itemName, String description, double price, String category, int quantity)
        {
            if (itemName.Equals("") || price <= 0 || category.Equals(""))
                return new Failure("One Or More Of The New Item Details Are Invalid");
            // checks that the item exists
            if (storage.ContainsKey(itemID))
            {
                Item item = storage[itemID];
                item.itemName = itemName;
                item.description = description;
                item.price = price;
                item.category = category;
                if (item.setQuantity(quantity))
                    return new Ok("Item Details Have Been Successfully Updated In The Store");
                return new Failure("Item quantity can't be negative");
            }
            return new Failure("Item Not Exist In Storage");
        }

        // add a new purchase prediacte for the store
        public int addPurchasePredicate(Predicate<PurchaseDetails> newPredicate, String predDescription)
        {
            return this.purchasePolicy.addPurchasePredicate(newPredicate, predDescription);
        }

        // removes purchase prediacte from the store
        public RegularResult removePurchasePredicate(int predicateID)
        {
            return this.purchasePolicy.removePurchasePredicate(predicateID);
        }

        // compose two predicates by the type of predicate 
        public ResultWithValue<int> composePurchasePredicates(int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition)
        {
            return this.purchasePolicy.composePurchasePredicates(firstPredicateID, secondPredicateID, typeOfComposition);
        }

        // returns the descriptions of all preds for presenting them to the user
        public ConcurrentDictionary<int, String> getPurchasePredicatesDescriptions()
        {
            return this.purchasePolicy.getPurchasePredicatesDescriptions();
        }

        // add new sale for the store sale policy
        public int addSale(int salePercentage, ApplySaleOn saleOn, String saleDescription)
        {
            return this.salesPolicy.addSale(salePercentage, saleOn, saleDescription);
        }

        // remove sale from the store sale policy
        public RegularResult removeSale(int saleID)
        {
            return this.salesPolicy.removeSale(saleID);
        }

        // add conditional for getting the sale
        public ResultWithValue<int> addSaleCondition(int saleID, SimplePredicate condition, SalePredicateCompositionType compositionType)
        {
            return this.salesPolicy.addSaleCondition(saleID, condition, compositionType);
        }

        // compose two sales by the type of sale 
        public ResultWithValue<int> composeSales(int firstSaleID, int secondSaleID, SaleCompositionType typeOfComposition, SimplePredicate selectionRule)
        {
            return this.salesPolicy.composeSales(firstSaleID, secondSaleID, typeOfComposition, selectionRule);
        }

        // returns the descriptions of all sales for presenting them to the user
        public ConcurrentDictionary<int, String> getSalesDescriptions()
        {
            return this.salesPolicy.getSalesDescriptions();
        }

        // Apply the sales policy on a list of items
        // The function will return the price after discount for each of the items
        // <itemID, price>
        public ConcurrentDictionary<int, double> applySalesPolicy(ConcurrentDictionary<Item, int> items, PurchaseDetails purchaseDetails)
        {
            return salesPolicy.pricesAfterSalePolicy(items, purchaseDetails);
        }

        // Apply the purchase policy on a list of items and their type of purchase
        // The function will return if the purchase can be made
        public RegularResult applyPurchasePolicy(PurchaseDetails purchaseDetails)
        {
            return purchasePolicy.approveByPurchasePolicy(purchaseDetails);
        }

        // purchase the items if the purchase request suitable with the store's policy and the products are also in stock
        public ResultWithValue<double> purchaseItems(User buyer, ConcurrentDictionary<int, int> items, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            ResultWithValue<ConcurrentDictionary<Item, int>> objItemsRes = getObjItems(items);
            if (!objItemsRes.getTag())
            {
                return new FailureWithValue<double>(objItemsRes.getMessage(), -1);
            }
            PurchaseDetails purchaseDetails = new PurchaseDetails(buyer, objItemsRes.getValue(), itemsPurchaseType);

            // checks the purchase can be made by the purchase policy
            RegularResult purchasePolicyRes = applyPurchasePolicy(purchaseDetails);
            if (purchasePolicyRes.getTag())
            {
                // checks that all the items available in the store storage
                RegularResult availableItemsRes = purchaseItemsIfAvailable(items);
                if (availableItemsRes.getTag())
                {
                    // calculate the price of each item after sales
                    ConcurrentDictionary<int, double> pricesAfterSaleRes = applySalesPolicy(objItemsRes.getValue(), purchaseDetails);
                    double totalPrice = 0;
                    foreach (KeyValuePair<int, double> itemPrice in pricesAfterSaleRes)
                    {
                        totalPrice += itemPrice.Value * items[itemPrice.Key];   // item price multiple by his quantity in the purchase
                    }
                    return new OkWithValue<double>("The Purchase Can Be Made, The Items Are Available In Storage And The Final Price Calculated For Each Item", totalPrice);
                }
                return new FailureWithValue<double>(availableItemsRes.getMessage(), -1);
            }
            return new FailureWithValue<double>(purchasePolicyRes.getMessage(), -1);
        }

        private ResultWithValue<ConcurrentDictionary<Item, int>> getObjItems(ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<Item, int> objItems = new ConcurrentDictionary<Item, int>();
            foreach (KeyValuePair<int, int> item in items)
            {
                int itemID = item.Key;
                if (!storage.ContainsKey(itemID))
                {
                    return new FailureWithValue<ConcurrentDictionary<Item, int>>("One Or More Of The Items Not Exist In Storage", null);
                }
                Item objItem = storage[itemID];
                objItems.TryAdd(objItem, item.Value);
            }
            return new OkWithValue<ConcurrentDictionary<Item, int>>("All Items Exist In Storage", objItems);
        }

        // Purchase items from a store if all items are available in storage
        // The purchase of the items updates the quantity of the items in storage
        public RegularResult purchaseItemsIfAvailable(ConcurrentDictionary<int, int> items)
        {
            ConcurrentDictionary<int, int> updatedItems = new ConcurrentDictionary<int, int>();
            lock (purchaseItemsLock)
            {
                foreach (KeyValuePair<int, int> item in items)
                {
                    int itemID = item.Key;
                    int quantity = item.Value;
                    RegularResult itemAvailableRes = isAvailableInStorage(itemID, quantity);
                    if (itemAvailableRes.getTag())
                    {
                        RegularResult changeQuantityRes = changeItemQuantity(itemID, -1 * quantity);
                        if(changeQuantityRes.getTag())
                        {
                            updatedItems.TryAdd(itemID, quantity);
                        }
                        else
                        {
                            rollBackPurchase(updatedItems);   // if at least one of the items not available, the purchase is canceled
                            return new Failure("One Or More Of The " + changeQuantityRes.getMessage());
                        }
                    }
                    else
                    {
                        rollBackPurchase(updatedItems);   // if at least one of the items not available, the purchase is canceled
                        return new Failure("One Or More Of " + itemAvailableRes.getMessage());
                    }
                }
                return new Ok("All Items Are Available In The Store's Storage");
            }
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
        public RegularResult deliverItems(String address, ConcurrentDictionary<int, int> items)
        {
            return this.deliverySystem.deliverItems(address, items);
        }

        // Adds a new store seller for this store
        public RegularResult addNewStoreSeller(SellerPermissions sellerPermissions)
        {
            lock (addStoreSellerLock)
            {
                String sellerUserName = sellerPermissions.seller.userName;
                if (!storeSellersPermissions.ContainsKey(sellerUserName))
                {
                    storeSellersPermissions.TryAdd(sellerUserName, sellerPermissions);
                    return new Ok("The New Store Seller Added To The Store Successfully");
                }
                return new Failure("The Store Seller Is Already Defined As A Seller In This Store");
            }
        }

        // Removes a store seller from this store
        public RegularResult removeStoreSeller(String sellerUserName)
        {
            if (storeSellersPermissions.ContainsKey(sellerUserName))
            {
                storeSellersPermissions.TryRemove(sellerUserName, out _);
                return new Ok("The Store Seller Removed From The Store Successfully");
            }
            return new Failure("The Store Seller Is Not Defined As A Seller In This Store");
        }

        // Removes a store seller from this store
        public RegularResult removeStoreOwner(String sellerUserName)
        {
            if (storeSellersPermissions.ContainsKey(sellerUserName))
            {
                storeSellersPermissions.TryRemove(sellerUserName, out _);
                foreach(KeyValuePair<string,SellerPermissions> val in storeSellersPermissions)
                {
                    if (val.Value.grantor.userName.Equals(sellerUserName))
                    {
                        storeSellersPermissions.TryRemove(val.Key, out _);
                    }
                }
                return new Ok("The Store Seller Removed From The Store Successfully");
            }
            return new Failure("The Store Seller Is Not Defined As A Seller In This Store");
        }


        // checks if the user is seller in this store
        // if he is, returns his seller permissions in the store
        public ResultWithValue<SellerPermissions> getStoreSellerPermissions(String userName)
        {
            if(storeSellersPermissions.ContainsKey(userName))
            {
                return new OkWithValue<SellerPermissions>("The User Is Seller In This Store", storeSellersPermissions[userName]);
            }
            return new FailureWithValue<SellerPermissions>("The User Is Not Seller In This Store", null);
        }

        // Returns information about all the store official (store owner and manager) in the store
        public ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> getStoreOfficialsInfo()
        {
            ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> officialsInfo = new ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>();
            foreach (KeyValuePair<string, SellerPermissions> sellerEntry in storeSellersPermissions)
            {
                SellerPermissions seller = sellerEntry.Value;
                officialsInfo.TryAdd(seller.seller.userName, seller.permissionsInStore);
            }
            return officialsInfo;
        }

        // Add purchase made by user in the store
        public void addNewPurchase(PurchaseInvoice purchase)
        {
            purchasesHistory.Add(purchase);
        }
    }
}
