using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public abstract class UserState
    {
        public User user { get; set; }

        public UserState(User user)
        {
            this.user = user;
        }

        public abstract UserType getUserType();
        public abstract RegularResult register(String userName, String password);
        public abstract RegularResult login(String userName, String password);
        
        public abstract RegularResult loginAsSystemManager(String userName, String password);
        public abstract RegularResult logout(String userName);

        public RegularResult addItemToShoppingCart(int storeID, int itemID, int quantity)
        {
            return this.user.shoppingCart.addItemToShoppingBag(storeID, itemID, quantity);
            // adding a quantity of the item to the shopping bag that belongs to the store id
        }

        public RegularResult removeItemFromShoppingCart(int storeID, int itemID)
        {
            return this.user.shoppingCart.removeItemFromShoppingBag(storeID, itemID);

        }

        public RegularResult purchaseItems(string address)
        {
            return HandlePurchases.Instance.purchaseItems(this.user, address); // handling the purchase procedure
        }

        public abstract ResultWithValue<int> openStore(String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalePolicy salesPolicy);
        public abstract RegularResult itemReview(String review, int itemID, int storeID);
        public abstract ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category);
        public abstract RegularResult removeItemFromStorage(int storeID, int itemID);
        public abstract RegularResult editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category);
        public abstract RegularResult appointStoreManager(String managerName, int storeID); //the store manager will receive default permissions(4.9)
        public abstract RegularResult appointStoreOwner(String storeOwnerName, int storeID);
        public abstract RegularResult editManagerPermissions(String managerName, ConcurrentLinkedList<Permissions> permissions, int storeID);
        public abstract RegularResult removeStoreManager(String managerName, int storeID);
        public abstract RegularResult removeStoreOwner(String ownerName, int storeID);
        public abstract ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID);
        public abstract ConcurrentBag<PurchaseInvoice> getStorePurchaseHistory(int storeID); //all the purchases of the store that I manage/own
        public abstract ConcurrentDictionary<String, ConcurrentBag<PurchaseInvoice>> getUsersPurchaseHistory();
        public abstract ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>> getStoresPurchaseHistory();
    }
}
