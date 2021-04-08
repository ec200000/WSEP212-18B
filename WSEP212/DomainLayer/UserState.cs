using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.Result;

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
        public abstract RegularResult logout(String userName);

        public abstract RegularResult addItemToShoppingCart(int storeID, int itemID, int quantity);
        public abstract RegularResult removeItemFromShoppingCart(int storeID, int itemID);
        //edit item in shopping cart is equal to -> remove + add
        public abstract RegularResult purchaseItems(string address); //later
    
        public abstract RegularResult openStore(String storeName, String storeAddress, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy);
        public abstract RegularResult itemReview(String review, int itemID, int storeID);
        public abstract ResultWithValue<int> addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category);
        public abstract RegularResult removeItemFromStorage(int storeID, int itemID);
        public abstract RegularResult editItemDetails(int storeID, int itemID, int quantity, String itemName, String description, double price, String category);
        public abstract RegularResult appointStoreManager(String managerName, int storeID); //the store manager will receive default permissions(4.9)
        public abstract RegularResult appointStoreOwner(String storeOwnerName, int storeID);
        public abstract RegularResult editManagerPermissions(String managerName, ConcurrentLinkedList<Permissions> permissions, int storeID);
        public abstract RegularResult removeStoreManager(String managerName, int storeID);
        public abstract ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID);
        public abstract ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID); //all the purchases of the store that I manage/own
        public abstract ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory();
        public abstract ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory();
    }
}
