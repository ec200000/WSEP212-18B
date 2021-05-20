using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public class ShoppingCart
    {
        [JsonIgnore]
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        // A data structure associated with a store ID and its shopping cart for a customer
        public ConcurrentDictionary<int, ShoppingBag> shoppingBags { get; set; }
        
        public string BagsAsJson
        {
            get => JsonConvert.SerializeObject(shoppingBags,settings);
            set => shoppingBags = JsonConvert.DeserializeObject<ConcurrentDictionary<int, ShoppingBag>>(value);
        }

        [Key]
        public string cartOwner { get; set; } 
        public ShoppingCart(string userName)
        {
            this.shoppingBags = new ConcurrentDictionary<int, ShoppingBag>();
            this.cartOwner = userName;
            SystemDBAccess.Instance.Carts.Add(this);
            SystemDBAccess.Instance.SaveChanges();
        }
        
        public ShoppingCart()
        {
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
                        return new Failure("Could not add items to the shopping bag!");
                    }
                    var result = SystemDBAccess.Instance.Carts.SingleOrDefault(c => c.cartOwner.Equals(this.cartOwner));
                    if (result != null)
                    {
                        if(!JToken.DeepEquals(result.BagsAsJson, this.BagsAsJson))
                            result.BagsAsJson = this.BagsAsJson;
                        SystemDBAccess.Instance.SaveChanges();
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
                var result = SystemDBAccess.Instance.Carts.SingleOrDefault(c => c.cartOwner.Equals(this.cartOwner));
                if (result != null)
                {
                    if(!JToken.DeepEquals(result.BagsAsJson, this.BagsAsJson))
                        result.BagsAsJson = this.BagsAsJson;
                    SystemDBAccess.Instance.SaveChanges();
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
                    var result = SystemDBAccess.Instance.Carts.SingleOrDefault(c => c.cartOwner.Equals(this.cartOwner));
                    if (result != null)
                    {
                        if(!JToken.DeepEquals(result.BagsAsJson, this.BagsAsJson))
                            result.BagsAsJson = this.BagsAsJson;
                        SystemDBAccess.Instance.SaveChanges();
                    }
                    return changeQuantityRes;
                }
                return new Failure(shoppingBagRes.getMessage());
            }
            return new Failure("Cannot Change Item Quantity To A Non-Positive Number");
        }

        // purchase all the items in the shopping cart
        // returns the total price after sales for each store. if the purchase cannot be made returns null
        public ResultWithValue<ConcurrentDictionary<int, double>> purchaseItemsInCart(User user, ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseType>> itemsPurchaseType)
        {
            if(isEmpty())
            {
                return new FailureWithValue<ConcurrentDictionary<int, double>>("Purchase Cannot Be Made When The Shopping Cart Is Empty", null);
            }
            else
            {
                ConcurrentDictionary<int, double> bagsFinalPrices = new ConcurrentDictionary<int, double>();
                ConcurrentDictionary<int, ShoppingBag> rollBackBags = new ConcurrentDictionary<int, ShoppingBag>();
                ResultWithValue<double> shoppingBagPriceRes;
                foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBags)
                {
                    int storeID = shoppingBag.Value.store.storeID;
                    if (itemsPurchaseType.TryGetValue(storeID, out ConcurrentDictionary<int, PurchaseType> bagItemsPurchaseType))
                    {
                        shoppingBagPriceRes = shoppingBag.Value.purchaseItemsInBag(user, bagItemsPurchaseType);
                        if (!shoppingBagPriceRes.getTag())
                        {
                            rollBackItemsInBags(rollBackBags);   // cancel the previous shopping cart items
                            return new FailureWithValue<ConcurrentDictionary<int, double>>(shoppingBagPriceRes.getMessage(), null);
                        }
                        bagsFinalPrices.TryAdd(storeID, shoppingBagPriceRes.getValue());
                        rollBackBags.TryAdd(shoppingBag.Key, shoppingBag.Value);
                    }
                    else
                    {
                        rollBackItemsInBags(rollBackBags);   // cancel the previous shopping cart items
                        return new FailureWithValue<ConcurrentDictionary<int, double>>("No Purchase Type Was Selected For One Or More Of The Shopping Bag Items", null);
                    }
                }
                var result = SystemDBAccess.Instance.Carts.SingleOrDefault(c => c.cartOwner.Equals(this.cartOwner));
                if (result != null)
                {
                    if(!JToken.DeepEquals(result.BagsAsJson, this.BagsAsJson))
                        result.BagsAsJson = this.BagsAsJson;
                    SystemDBAccess.Instance.SaveChanges();
                }
                return new OkWithValue<ConcurrentDictionary<int, double>>("The Purchase Can Be Made, The Items Are Available In Storage And The Final Price Calculated For Each Item", bagsFinalPrices);
            }
        }

        // cancel the purchase in the other store - returns the items back to storage
        private void rollBackItemsInBags(ConcurrentDictionary<int, ShoppingBag> shoppingBagsToRollBack)
        {
            foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBagsToRollBack)
            {
                shoppingBag.Value.rollBackItems();
            }
            var result = SystemDBAccess.Instance.Carts.SingleOrDefault(c => c.cartOwner.Equals(this.cartOwner));
            if (result != null)
            {
                if(!JToken.DeepEquals(result.BagsAsJson, this.BagsAsJson))
                    result.BagsAsJson = this.BagsAsJson;
                SystemDBAccess.Instance.SaveChanges();
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
