using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer;

namespace WSEP212.ServiceLayer
{
    public interface ISystemController
    {
        public bool register(String userName, String password);
        public bool login(String userName, String password);
        public bool logout(String userName);

        public bool addItemToShoppingCart(String userName, int storeID, int itemID);
        public bool removeItemFromShoppingCart(String userName, int storeID, int itemID);
        //edit item in shopping cart is equal to -> remove + add
        public bool purchaseItems(String userName); //later
        public bool openStore(String userName, String storeName, String purchasePolicy, String salesPolicy);
        public bool itemReview(String userName, String review, int itemID, int storeID);
        public bool addItemToStorage(String userName, int storeID, ItemDTO item);
        public bool removeItemFromStorage(String userName, int storeID, int itemID);
        public bool editItemDetails(String userName, int storeID, ItemDTO item);
        public bool appointStoreManager(String userName, String managerName, int storeID); //the store manager will receive default permissions(4.9)
        public bool appointStoreOwner(String userName, String storeOwnerName, int storeID);
        public bool editManagerPermissions(String userName, String managerName, ConcurrentBag<int> permissions, int storeID);
        public bool removeStoreManager(String userName, String managerName, int storeID);
        public ConcurrentDictionary<String, ConcurrentBag<Permissions>> getOfficialsInformation(String userName, int storeID);
        public ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(String userName, int storeID); //all the purchases of the store that I manage/own
        public ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory(String userName);
        public ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory(String userName);
    }
}
