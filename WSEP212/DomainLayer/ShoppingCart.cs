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
        public RegularResult addItemToShoppingBag(int storeID, int itemID, int quantity)
        {
            if (quantity > 0)
            {
                ResultWithValue<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
                if (shoppingBagRes.getTag())
                {
                    RegularResult addItemRes = shoppingBagRes.getValue().addItem(itemID, quantity);
                    if (!addItemRes.getTag())
                    {
                        removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                    }
                    return addItemRes;
                }
                return new Failure(shoppingBagRes.getMessage());
            }
            return new Failure("Cannot Add A Item To The Shopping Bag With A Non-Positive Quantity");
        }

        // Removes a item from a store's shopping bag if the store and the item exists 
        // If the operation was successful, remove the shopping bag if it is empty
        public RegularResult removeItemFromShoppingBag(int storeID, int itemID)
        {
            ResultWithValue<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
            if (shoppingBagRes.getTag())
            {
                RegularResult removeItemRes = shoppingBagRes.getValue().removeItem(itemID);
                if (removeItemRes.getTag())
                {
                    removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                }
                return removeItemRes;
            }
            return new Failure(shoppingBagRes.getMessage());
        }

        // Changes the quantity of item in a shopping bag
        // If the operation was successful, remove the shopping bag if it is empty
        public RegularResult changeItemQuantityInShoppingBag(int storeID, int itemID, int updatedQuantity)
        {
            if (updatedQuantity >= 0)
            {
                ResultWithValue<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
                if (shoppingBagRes.getTag())
                {
                    RegularResult changeQuantityRes = shoppingBagRes.getValue().changeItemQuantity(itemID, updatedQuantity);
                    if (changeQuantityRes.getTag())
                    {
                        removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                    }
                    return changeQuantityRes;
                }
                return new Failure(shoppingBagRes.getMessage());
            }
            return new Failure("Cannot Change Item Quantity To A Non-Positive Number");
        }

        // purchase all the items in the shopping cart
        // returns the total price after sales. if the purchase cannot be made returns -1
        public ResultWithValue<double> purchaseItemsInCart(User user, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            if(isEmpty())
            {
                return new FailureWithValue<double>("Purchase Cannot Be Made When The Shopping Cart Is Empty", -1);
            }
            else
            {
                double totalPrice = 0;
                ResultWithValue<double> shoppingBagPriceRes;
                foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBags)
                {
                    shoppingBagPriceRes = shoppingBag.Value.purchaseItemsInBag(user, itemsPurchaseType);
                    if (!shoppingBagPriceRes.getTag())
                    {
                        return shoppingBagPriceRes;
                    }
                    totalPrice += shoppingBagPriceRes.getValue();
                }
                return new OkWithValue<double>("The Purchase Can Be Made, The Items Are Available In Storage And The Final Price Calculated For Each Item", totalPrice);
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
        private ResultWithValue<ShoppingBag> getStoreShoppingBag(int storeID)
        {
            if (shoppingBags.ContainsKey(storeID))
            {
                return new OkWithValue<ShoppingBag>("Returns An Existing Shopping Bag", shoppingBags[storeID]);
            }
            else
            {
                ResultWithValue<Store> storeRes = StoreRepository.Instance.getStore(storeID);
                if (storeRes.getTag())
                {
                    ShoppingBag newShoppingBag = new ShoppingBag(storeRes.getValue());
                    shoppingBags.TryAdd(storeID, newShoppingBag);
                    return new OkWithValue<ShoppingBag>("Returns A New Shopping Bag", newShoppingBag);
                }
                return new FailureWithValue<ShoppingBag>(storeRes.getMessage(), null);
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
