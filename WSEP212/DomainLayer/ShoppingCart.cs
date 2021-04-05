using System.Collections.Concurrent;
using System.Collections.Generic;

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
        public bool addItemToShoppingBag(int storeID, int itemID, int quantity)
        {
            bool addItem = false;
            if (quantity > 0)
            {
                ShoppingBag shoppingBag = getStoreShoppingBag(storeID);
                if (shoppingBag != null)
                {
                    addItem = shoppingBag.addItem(itemID, quantity);
                    if (!addItem)
                    {
                        removeShoppingBagIfEmpty(shoppingBag);
                    }
                }
            }
            return addItem;
        }

        // Removes a item from a store's shopping bag if the store and the item exists 
        // If the operation was successful, remove the shopping bag if it is empty
        public bool removeItemFromShoppingBag(int storeID, int itemID)
        {
            ShoppingBag shoppingBag = getStoreShoppingBag(storeID);
            bool removeItem = false;

            if (shoppingBag != null)
            {
                removeItem = shoppingBag.removeItem(itemID);
                if (removeItem)
                {
                    removeShoppingBagIfEmpty(shoppingBag);
                }
            }
            return removeItem;
        }

        // Changes the quantity of item in a shopping bag
        // If the operation was successful, remove the shopping bag if it is empty
        public bool changeItemQuantityInShoppingBag(int storeID, int itemID, int updatedQuantity)
        {
            bool changeQuantity = false;
            if (updatedQuantity >= 0)
            {
                ShoppingBag shoppingBag = getStoreShoppingBag(storeID);
                if (shoppingBag != null)
                {
                    changeQuantity = shoppingBag.changeItemQuantity(itemID, updatedQuantity);
                    if (changeQuantity)
                    {
                        removeShoppingBagIfEmpty(shoppingBag);
                    }
                }
            }
            return changeQuantity;
        }

        // purchase all the items in the shopping cart
        // returns the total price after sales. if the purchase cannot be made returns -1
        public double purchaseItemsInCart(User user, ConcurrentDictionary<int, PurchaseType> itemsPurchaseType)
        {
            double totalPrice = 0;
            foreach (KeyValuePair<int, ShoppingBag> shoppingBag in shoppingBags)
            {
                double shoppingBagPrice = shoppingBag.Value.purchaseItemsInBag(user, itemsPurchaseType);
                if(shoppingBagPrice < 0)
                {
                    return -1;
                }
                totalPrice += shoppingBagPrice;
            }
            return totalPrice;
        }

        // Removes all the shopping bags in the shopping cart
        public void clearShoppingCart()
        {
            shoppingBags.Clear();
        }

        // Returns the store's shopping bag
        // If the shopping bag does not exist, create a new shopping bag for the relevent store
        // If the store does not exist/active we will return null 
        private ShoppingBag getStoreShoppingBag(int storeID)
        {
            if (shoppingBags.ContainsKey(storeID))
            {
                return shoppingBags[storeID];
            }
            else
            {
                Store store = StoreRepository.Instance.getStore(storeID);
                ShoppingBag newShoppingBag = null;
                if (store != null)
                {
                    newShoppingBag = new ShoppingBag(store);
                    shoppingBags.TryAdd(storeID, newShoppingBag);
                }
                return newShoppingBag;
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
