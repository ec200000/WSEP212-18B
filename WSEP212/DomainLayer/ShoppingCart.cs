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

        public ShoppingCart(User cartOwner)
        {
            this.cartOwner = cartOwner;
            this.shoppingBags = new ConcurrentDictionary<int, ShoppingBag>();
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
                return changePurchaseRes;
            }
            return new Failure(shoppingBagRes.getMessage());
        }

        // submit new item price offer
        public RegularResult submitPriceOffer(int storeID, int itemID, double offerItemPrice)
        {
            ResultWithValue<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
            if (shoppingBagRes.getTag())
            {
                RegularResult submitOfferRes = shoppingBagRes.getValue().submitItemPriceOffer(itemID, offerItemPrice);
                removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                return submitOfferRes;
            }
            return new Failure(shoppingBagRes.getMessage());
        }

        // update the status of the price - can be done only by store owners & managers
        public RegularResult updatePriceStatus(int storeID, int itemID, PriceStatus priceStatus)
        {
            ResultWithValue<ShoppingBag> shoppingBagRes = getStoreShoppingBag(storeID);
            if (shoppingBagRes.getTag())
            {
                RegularResult updateStatusRes = shoppingBagRes.getValue().updateItemPriceStatus(itemID, priceStatus);
                removeShoppingBagIfEmpty(shoppingBagRes.getValue());
                return updateStatusRes;
            }
            return new Failure(shoppingBagRes.getMessage());
        }

        // purchase all the items in the shopping cart
        // returns the total price after sales for all stores. if the purchase cannot be made returns -1
        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> purchaseItemsInCart()
        {
            if(isEmpty())
            {
                return new FailureWithValue<ConcurrentDictionary<int, PurchaseInvoice>>("Purchase Cannot Be Made When The Shopping Cart Is Empty", null);
            }
            else
            {
                ConcurrentDictionary<int, PurchaseInvoice> purchaseInvoices = new ConcurrentDictionary<int, PurchaseInvoice>();
                ResultWithValue<PurchaseInvoice> shoppingBagPriceRes;
                foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBags)
                {
                    int storeID = shoppingBag.Value.store.storeID;
                    shoppingBagPriceRes = shoppingBag.Value.purchaseItemsInBag();
                    if (!shoppingBagPriceRes.getTag())
                    {
                        rollBackBags(purchaseInvoices);   // cancel the previous shopping cart items
                        return new FailureWithValue<ConcurrentDictionary<int, PurchaseInvoice>>(shoppingBagPriceRes.getMessage(), null);
                    }
                    purchaseInvoices.TryAdd(storeID, shoppingBagPriceRes.getValue());
                }
                return new OkWithValue<ConcurrentDictionary<int, PurchaseInvoice>>("The Purchase Can Be Made, The Items Are Available In Storage And The Final Price Calculated For Each Item", purchaseInvoices);
            }
        }

        // calculate the final price of each item in the cart, after sales
        // returns <storeID, <itemID, <priceAfterSale, status>>>
        public ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>> getItemsAfterSalePrices()
        {
            ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>> pricesAfterSale = new ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>();
            ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>> bagAfterSale;
            foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBags)
            {
                bagAfterSale = shoppingBag.Value.getItemsAfterSalePrices();
                pricesAfterSale.TryAdd(shoppingBag.Key, bagAfterSale);
            }
            return pricesAfterSale;
        }

        // cancel the purchase in the other store - returns the items back to storage
        public void rollBackBags(ConcurrentDictionary<int, PurchaseInvoice> shoppingBagsToRollBack)
        {
            int storeID;
            foreach (KeyValuePair<int, PurchaseInvoice> storeInvoicePair in shoppingBagsToRollBack)
            {
                storeID = storeInvoicePair.Key;
                if (shoppingBags.ContainsKey(storeID))
                {
                    shoppingBags[storeID].rollBackPurchase(storeInvoicePair.Value.purchaseInvoiceID);
                }
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
