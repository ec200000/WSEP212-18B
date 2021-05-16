using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class ShoppingCart
    {
        public User cartOwner { get; set; }
        // A data structure associated with a store ID and its shopping cart for a customer
        public ConcurrentDictionary<int, ShoppingBag> shoppingBags { get; set; }
        public double cartFinalPrice { get; set; }

        public ShoppingCart(User cartOwner)
        {
            this.cartOwner = cartOwner;
            this.shoppingBags = new ConcurrentDictionary<int, ShoppingBag>();
            this.cartFinalPrice = 0;
        }

        // return true if the shopping cart is empty
        public bool isEmpty()
        {
            return shoppingBags.Count == 0;
        }

        // Adds a quantity items to a store's shopping bag if the store and the item exists 
        // If the operation fails, remove the shopping bag if it is empty
        public RegularResult addItemToShoppingBag(int storeID, int itemID, int quantity, ItemPurchaseType purchaseType)
        {
            if (quantity > 0)
            {
                ResultWithValue<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
                if (shoppingBagRes.getTag())
                {
                    RegularResult addItemRes = shoppingBagRes.getValue().addItem(itemID, quantity, purchaseType);
                    removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                    updateFinalCartPrice();
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
                removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                updateFinalCartPrice();
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
                    removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                    updateFinalCartPrice();
                    return changeQuantityRes;
                }
                return new Failure(shoppingBagRes.getMessage());
            }
            return new Failure("Cannot Change Item Quantity To A Non-Positive Number");
        }

        // Changes the purchase type of item in a shopping bag
        public RegularResult changeItemPurchaseTypeInShoppingBag(int storeID, int itemID, ItemPurchaseType updatedPurchaseType)
        {
            ResultWithValue<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
            if (shoppingBagRes.getTag())
            {
                RegularResult changePurchaseRes = shoppingBagRes.getValue().changeItemPurchaseType(itemID, updatedPurchaseType);
                removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                updateFinalCartPrice();
                return changePurchaseRes;
            }
            return new Failure(shoppingBagRes.getMessage());
        }

        // purchase all the items in the shopping cart
        // returns the total price after sales for all stores. if the purchase cannot be made returns -1
        public ResultWithValue<double> purchaseItemsInCart()
        {
            if(isEmpty())
            {
                return new FailureWithValue<double>("Purchase Cannot Be Made When The Shopping Cart Is Empty", -1);
            }
            else
            {
                ConcurrentDictionary<int, ShoppingBag> rollBackBags = new ConcurrentDictionary<int, ShoppingBag>();
                ResultWithValue<double> shoppingBagPriceRes;
                double totalPurchasePrice = 0;
                foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBags)
                {
                    int storeID = shoppingBag.Value.store.storeID;
                    shoppingBagPriceRes = shoppingBag.Value.purchaseItemsInBag();
                    if (!shoppingBagPriceRes.getTag())
                    {
                        rollBackItemsInBags(rollBackBags);   // cancel the previous shopping cart items
                        return new FailureWithValue<double>(shoppingBagPriceRes.getMessage(), -1);
                    }
                    totalPurchasePrice += shoppingBagPriceRes.getValue();
                    rollBackBags.TryAdd(shoppingBag.Key, shoppingBag.Value);
                }
                // calculate the sum of all stores prices
                this.cartFinalPrice = totalPurchasePrice;
                return new OkWithValue<double>("The Purchase Can Be Made, The Items Are Available In Storage And The Final Price Calculated For Each Item", totalPurchasePrice);
            }
        }

        // calculate the final price of the cart - after sales
        public void updateFinalCartPrice()
        {
            double updatedFinalPrice = 0;
            foreach (ShoppingBag shoppingBag in shoppingBags.Values)
            {
                updatedFinalPrice += shoppingBag.bagFinalPrice;
            }
            this.cartFinalPrice = updatedFinalPrice;
        }

        // create purchase invoices after paying for the items
        public void createPurchaseInvoices()
        {
            foreach (ShoppingBag shoppingBag in shoppingBags.Values)
            {
                shoppingBag.createPurchaseInvoice();
            }
        }

        // cancel the purchase in the other store - returns the items back to storage
        private void rollBackItemsInBags(ConcurrentDictionary<int, ShoppingBag> shoppingBagsToRollBack)
        {
            foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBagsToRollBack)
            {
                shoppingBag.Value.rollBackItems();
            }
        }

        // returns all the items in the shopping bags to the stores storage
        public void rollBackShoppingCart()
        {
            rollBackItemsInBags(shoppingBags);
        }

        // Removes all the shopping bags in the shopping cart
        public void clearShoppingCart()
        {
            shoppingBags.Clear();
            cartFinalPrice = 0;
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
                    ShoppingBag newShoppingBag = new ShoppingBag(storeRes.getValue(), cartOwner);
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
