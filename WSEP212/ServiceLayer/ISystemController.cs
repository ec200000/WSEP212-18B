using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.Result;

namespace WSEP212.ServiceLayer
{
    public interface ISystemController
    {
        public RegularResult register(String userName, String password);
        public RegularResult login(String userName, String password);
        public RegularResult logout(String userName);

        public RegularResult addItemToShoppingCart(String userName, int storeID, int itemID, int quantity);
        public RegularResult removeItemFromShoppingCart(String userName, int storeID, int itemID);
        //edit item in shopping cart is equal to -> remove + add
        public RegularResult purchaseItems(String userName, String address);
        public ResultWithValue<int> openStore(String userName, String storeName, String storeAddress, String purchasePolicy, String salesPolicy);
        public RegularResult itemReview(String userName, String review, int itemID, int storeID);
        public ResultWithValue<int> addItemToStorage(String userName, int storeID, ItemDTO item);
        public RegularResult removeItemFromStorage(String userName, int storeID, int itemID);
        public RegularResult editItemDetails(String userName, int storeID, ItemDTO item);
        public RegularResult appointStoreManager(String userName, String managerName, int storeID); //the store manager will receive default permissions(4.9)
        public RegularResult appointStoreOwner(String userName, String storeOwnerName, int storeID);
        public RegularResult editManagerPermissions(String userName, String managerName, ConcurrentLinkedList<Int32> permissions, int storeID);
        public RegularResult removeStoreManager(String userName, String managerName, int storeID);
        public ResultWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>> getOfficialsInformation(String userName, int storeID);
        public ResultWithValue<ConcurrentBag<PurchaseInfo>> getStorePurchaseHistory(String userName, int storeID); //all the purchases of the store that I manage/own
        public ResultWithValue<ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>>> getUsersPurchaseHistory(String userName);
        public ResultWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>>> getStoresPurchaseHistory(String userName);
    }
}
