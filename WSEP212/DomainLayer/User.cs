using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class User
    {
        public String userName { get; set; }
        public UserState state { get; set; }
        public ShoppingCart shoppingCart { get; set; }
        public ConcurrentBag<PurchaseInfo> purchases { get; set; }
        public ConcurrentBag<SellerPermissions> sellerPermissions { get; set; }

        public User(String userName)
        {
            this.userName = userName;
        }

        public void changeState(UserState state)
        {

        }

        public bool register(String userName, String password)
        {
            return false;
        }

        public bool login(String userName, String password)
        {
            return false;
        }

        public bool logout(String userName)
        {
            return false;
        }

        public bool addItemToShoppingCart(int storeID, int itemID)
        {
            return false;
        }

        public bool removeItemFromShoppingCart(int storeID, int itemID)
        {
            return false;

        }
        //edit item in shopping cart is equal to -> remove + add
        public bool purchaseItems() //later
        {
            return false;
        }

        public bool openStore(String storeName, PurchasePolicy purchasePolicy, SalesPolicy salesPolicy)
        {
            return false;
        }

        public bool itemReview(String review, int itemID, int storeID)
        {
            return false;
        }

        public bool addItemToStorage(int storeID, Item item, int quantity)
        {
            return false;
        }

        public bool removeItemFromStorage(int storeID, Item item)
        {
            return false;
        }

        public bool editItemDetails(int storeID, Item item)
        {
            return false;
        }

        public bool appointStoreManager(String managerName, int storeID) //the store manager will receive default permissions(4.9)
        {
            return false;
        }

        public bool appointStoreOwner(String storeOwnerName, int storeID)
        {
            return false;
        }

        public bool editManagerPermissions(String managerName, ConcurrentBag<Permissions> permissions)
        {
            return false;
        }

        public bool removeStoreManager(String managerName, int storeID)
        {
            return false;
        }

        public ConcurrentDictionary<String, ConcurrentBag<Permissions>> getOfficialsInformation(int storeID)
        {
            return null;
        }

        public ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(int storeID) //all the purchases of the store that I manage/own
        {
            return null;
        }

        public ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory()
        {
            return null;
        }

        public ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory()
        {
            return null;
        }

    }
}
