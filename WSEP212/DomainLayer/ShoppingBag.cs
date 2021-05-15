using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class ShoppingBag
    {
        public Store store { get; set; }
        // A data structure associated with a item ID and its quantity - more effective when there will be a sales policy
        public ConcurrentDictionary<int, int> items { get; set; }
        public ConcurrentDictionary<int, ItemPurchaseType> itemsPurchaseTypes { get; set; }

        public ShoppingBag(Store store)
        {
            this.store = store;
            this.items = new ConcurrentDictionary<int, int>();
            this.itemsPurchaseTypes = new ConcurrentDictionary<int, ItemPurchaseType>();
        }

        // return true if the shopping bag is empty
        public bool isEmpty()
        {
            return items.Count == 0;
        }

        // Adds item to the shopping bag if the item exist and available in the store
        // quantity is the number of the same item to add
        public RegularResult addItem(int itemID, int quantity, ItemPurchaseType purchaseType)
        {
            if (quantity > 0)
            {
                if(items.ContainsKey(itemID))
                {
                    int updatedQuantity = quantity + items[itemID];
                    // check if the item quantity available in storage
                    RegularResult itemAvailableRes = store.isAvailableInStorage(itemID, updatedQuantity);
                    if (itemAvailableRes.getTag())
                    {
                        items[itemID] = updatedQuantity;
                        return new Ok("The Item Was Successfully Added To The Shopping Bag");
                    }
                    return itemAvailableRes;
                }
                else
                {
                    // check if the item quantity available in storage
                    RegularResult itemAvailableRes = store.isAvailableInStorage(itemID, quantity);
                    if (itemAvailableRes.getTag())
                    {
                        items.TryAdd(itemID, quantity);
                        itemsPurchaseTypes.TryAdd(itemID, purchaseType);
                        return new Ok("The Item Was Successfully Added To The Shopping Bag");
                    }
                    return itemAvailableRes;
                }
            }
            return new Failure("Cannot Add A Item To The Shopping Bag With A Non-Positive Quantity");
        }

        // Removes the item from the shopping bag if it exists
        // If the item has n quantity in the basket, all the n will be deleted
        public RegularResult removeItem(int itemID)
        {
            if(items.ContainsKey(itemID))
            {
                items.TryRemove(itemID, out _);
                return new Ok("The Item Was Successfully Removed From The Shopping Bag");
            }
            return new Failure("The Item Is Not Exist In The Shopping Bag");
        }

        // Changes the quantity of item in the bag if the item is in the bag and available in the store
        public RegularResult changeItemQuantity(int itemID, int updatedQuantity)
        {
            if(updatedQuantity == 0)
            {
                return removeItem(itemID);
            }
            else if(updatedQuantity > 0) 
            { 
                if (items.ContainsKey(itemID))  // check if item in the shopping bag
                {
                    RegularResult itemAvailableRes = store.isAvailableInStorage(itemID, updatedQuantity);
                    if (itemAvailableRes.getTag())   // check if item available in store
                    {
                        items[itemID] = updatedQuantity;
                        return new Ok("Item Quantity Was Successfully Changed In The Shopping Bag");
                    }
                    return itemAvailableRes;
                }
                return new Failure("The Item Is Not Exist In The Shopping Bag");
            }
            return new Failure("Cannot Change Item Quantity To A Non-Positive Number");
        }

        // purchase all the items in the shopping bag
        // returns the total price after sales. if the purchase cannot be made returns -1
        public ResultWithValue<double> purchaseItemsInBag(User user, ConcurrentDictionary<int, ItemPurchaseType> itemsPurchaseType)
        {
            foreach (KeyValuePair<int, int> item in items)
            {
                if(!itemsPurchaseType.ContainsKey(item.Key))
                {
                    return new FailureWithValue<double>("No Purchase Type Was Selected For One Or More Of The Items", -1);
                }
            }
            return store.purchaseItems(user, items, itemsPurchaseType);
        }

        // roll back purchase - returns all the items in the bag to the store
        public void rollBackItems()
        {
            store.rollBackPurchase(items);
        }

        // Removes all the items in the shopping bag
        public void clearShoppingBag()
        {
            items.Clear();
        }

    }
}
