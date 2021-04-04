using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

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
        public abstract bool register(String userName, String password);
        public abstract bool login(String userName, String password);
        public abstract bool logout(String userName);

        public abstract bool addItemToShoppingCart(int storeID, int itemID, int quantity);
        public abstract bool removeItemFromShoppingCart(int storeID, int itemID);
        //edit item in shopping cart is equal to -> remove + add
        public abstract bool purchaseItems(string address); //later
    
        public abstract bool openStore(String storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy);
        public abstract bool itemReview(String review, int itemID, int storeID);
        public abstract bool addItemToStorage(int storeID, Item item);
        public abstract bool removeItemFromStorage(int storeID, Item item);
        public abstract bool editItemDetails(int storeID, Item item);
        public abstract bool appointStoreManager(String managerName, int storeID); //the store manager will receive default permissions(4.9)
        public abstract bool appointStoreOwner(String storeOwnerName, int storeID);
        public abstract bool editManagerPermissions(String managerName, ConcurrentLinkedList<Permissions> permissions, int storeID);
        public abstract bool removeStoreManager(String managerName, int storeID);
        public abstract ConcurrentDictionary<User, ConcurrentLinkedList<Permissions>> getOfficialsInformation(int storeID);
        public abstract ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID); //all the purchases of the store that I manage/own
        public abstract ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory();
        public abstract ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory();
    }
}
