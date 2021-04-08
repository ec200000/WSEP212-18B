using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.DomainLayer.Result;

namespace WSEP212.DomainLayer
{
    public class ShoppingCart
    {
        // A data structure associated with a store ID and its shopping cart for a customer
        // There is no need for a structure that allows threads use, since only a single user can use these actions on his shopping cart
        public ConcurrentDictionary<int, ShoppingBag> shoppingBags { get; set; }

        public ShoppingCart()
        {
            this.shoppingBags = new ConcurrentDictionary<int, ShoppingBag>();
        }

        // return true if the shopping cart is empty
        public bool isEmpty()
        {
            return shoppingBags.Count == 0;
        }

        // Adds a quantity items to a store's shopping bag if the store and the item exists 
        // If the operation fails, remove the shopping bag if it is empty
        public Result<Object> addItemToShoppingBag(int storeID, int itemID, int quantity)
        {
            if (quantity > 0)
            {
                Result<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
                if (shoppingBagRes.getTag())
                {
                    Result<Object> addItemRes = shoppingBagRes.getValue().addItem(itemID, quantity);
                    if (!addItemRes.getTag())
                    {
                        removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                    }
                    return addItemRes;
                }
                return new Failure<Object>(shoppingBagRes.getMessage(), null);
            }
            return new Failure<Object>("Cannot Add A Item To The Shopping Bag With A Non-Positive Quantity", null);
        }

        // Removes a item from a store's shopping bag if the store and the item exists 
        // If the operation was successful, remove the shopping bag if it is empty
        public Result<Object> removeItemFromShoppingBag(int storeID, int itemID)
        {
            Result<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
            if (shoppingBagRes.getTag())
            {
                Result<Object> removeItemRes = shoppingBagRes.getValue().removeItem(itemID);
                if (removeItemRes.getTag())
                {
                    removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                }
                return removeItemRes;
            }
            return new Failure<Object>(shoppingBagRes.getMessage(), null);
        }

        // Changes the quantity of item in a shopping bag
        // If the operation was successful, remove the shopping bag if it is empty
        public Result<Object> changeItemQuantityInShoppingBag(int storeID, int itemID, int updatedQuantity)
        {
            if (updatedQuantity >= 0)
            {
                Result<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
                if (shoppingBagRes.getTag())
                {
                    Result<Object> changeQuantityRes = shoppingBagRes.getValue().changeItemQuantity(itemID, updatedQuantity);
                    if (changeQuantityRes.getTag())
                    {
                        removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                    }
                    return changeQuantityRes;
                }
                return new Failure<Object>(shoppingBagRes.getMessage(), null);
            }
            return new Failure<Object>("Cannot Change Item Quantity To A Non-Positive Number", null);
        }

        // purchase all the items in the shopping cart
        // returns the total price after sales. if the purchase cannot be made returns -1
        public Result<double> purchaseItemsInCart(User user, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            if(isEmpty())
            {
                return new Failure<double>("Purchase Cannot Be Made When The Shopping Cart Is Empty", -1);
            }
            else
            {
                double totalPrice = 0;
                Result<double> shoppingBagPriceRes;
                foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBags)
                {
                    shoppingBagPriceRes = shoppingBag.Value.purchaseItemsInBag(user, itemsPurchaseType);
                    if (!shoppingBagPriceRes.getTag())
                    {
                        return shoppingBagPriceRes;
                    }
                    totalPrice += shoppingBagPriceRes.getValue();
                }
                return new Ok<double>("The Purchase Can Be Made, The Items Are Available In Storage And The Final Price Calculated For Each Item", totalPrice);
            }
        }

        // Removes all the shopping bags in the shopping cart
        public void clearShoppingCart()
        {
            shoppingBags.Clear();
        }

        // Returns the store's shopping bag
        // If the shopping bag does not exist, create a new shopping bag for the relevent store
        // If the store does not exist/active we will return null 
        private Result<ShoppingBag> getStoreShoppingBag(int storeID)
        {
            if (shoppingBags.ContainsKey(storeID))
            {
                return new Ok<ShoppingBag>("Returns An Existing Shopping Bag", shoppingBags[storeID]);
            }
            else
            {
                Result<Store> storeRes = StoreRepository.Instance.getStore(storeID);
                if (storeRes.getTag())
                {
                    ShoppingBag newShoppingBag = new ShoppingBag(storeRes.getValue());
                    shoppingBags.TryAdd(storeID, newShoppingBag);
                    return new Ok<ShoppingBag>("Returns A New Shopping Bag", newShoppingBag);
                }
                return new Failure<ShoppingBag>(storeRes.getMessage(), null);
            }
        }

        // remove the shopping bag from the shopping cart if it is empty
        private void removeShoppingBagIfEmpty(ShoppingBag shoppingBag)
        {
            if(shoppingBag.isEmpty())
            {
                int storeID = shoppingBag.store.storeID;
                shoppingBags.TryRemove(storeID, out _);
            }
        }
    }
}
