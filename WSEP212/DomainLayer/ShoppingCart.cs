using System.Collections.Generic;

namespace WSEP212.DomainLayer
{
    public class ShoppingCart
    {
        // A data structure associated with a store ID and its shopping cart for a customer
        // There is no need for a structure that allows threads use, since only a single user can use these actions on his shopping cart
        public Dictionary<int, ShoppingBag> shoppingBags { get; set; }

        public ShoppingCart()
        {
            this.shoppingBags = new Dictionary<int, ShoppingBag>();
        }

        // Adds a quantity items to a store's shopping bag if the store and the item exists 
        // If the operation fails, remove the shopping bag if it is empty
        public bool addItemToShoppingBag(int storeID, int itemID, int quantity)
        {
            ShoppingBag shoppingBag = getStoreShoppingBag(storeID);
            bool addItem = false;

            if (shoppingBag != null)
            {
                addItem = shoppingBag.addItem(itemID, quantity);
                if(!addItem)
                {
                    removeShoppingBagIfEmpty(shoppingBag);
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
        public bool changeItemQuantityInShoppingBag(int storeID, int itemID, int quantity)
        {
            ShoppingBag shoppingBag = getStoreShoppingBag(storeID);
            bool changeQuantity = false;

            if (shoppingBag != null)
            {
                changeQuantity = shoppingBag.changeItemQuantity(itemID, quantity);
                if(changeQuantity)
                {
                    removeShoppingBagIfEmpty(shoppingBag);
                }
            }
            return changeQuantity;
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
                Store store = StoreRepository.getInstance().getStore(storeID);
                ShoppingBag newShoppingBag = null;
                if (store != null)
                {
                    newShoppingBag = new ShoppingBag(store);
                    shoppingBags.Add(storeID, newShoppingBag);
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
                shoppingBags.Remove(storeID);
            }
        }
    }
}
