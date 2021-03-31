using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using WSEP212.DomainLayer;

namespace WSEP212.ServiceLayer
{
    public class UserManager : IUserManager
    {
        public UserManagerFacade userManagerFacade { get; set; }

        public UserManager() { }

        public bool register(String userName, String password)
        {
            return userManagerFacade.register(userName, password);
        }
        public bool login(String userName, String password)
        {
            return userManagerFacade.login(userName, password);
        }
        public bool logout(String userName)
        {
            return userManagerFacade.logout(userName);
        }

        public bool addItemToShoppingCart(String userName, int storeID, int itemID)
        {
            return userManagerFacade.addItemToShoppingCart(userName, storeID, itemID);
        }
        public bool removeItemFromShoppingCart(String userName, int storeID, int itemID)
        {
            return userManagerFacade.removeItemFromShoppingCart(userName, storeID, itemID);
        }

        public bool purchaseItems(String userName)
        {
            return userManagerFacade.purchaseItems(userName);
        }
        public bool openStore(String userName, String storeName, String purchasePolicy, String salesPolicy)
        {
            PurchasePolicy newPurchasePolicy = new PurchasePolicy(purchasePolicy);
            SalesPolicy newSalesPolicy = new SalesPolicy(salesPolicy);
            return userManagerFacade.openStore(userName, storeName, newPurchasePolicy, newSalesPolicy);
        }
        public bool itemReview(String userName, String review, int itemID, int storeID)
        {
            return userManagerFacade.itemReview(userName, review, itemID, storeID);
        }
        public bool addItemToStorage(String userName, int storeID, ItemDTO item, int quantity)
        {
            Item newItem = new Item(item.quantity, item.itemName, item.description, item.price, item.category);
            return userManagerFacade.addItemToStorage(userName, storeID, newItem, quantity);
        }
        public bool removeItemFromStorage(String userName, int storeID, ItemDTO item)
        {
            Item newItem = new Item(item.quantity, item.itemName, item.description, item.price, item.category);
            return userManagerFacade.removeItemFromStorage(userName, storeID, newItem);
        }
        public bool editItemDetails(String userName, int storeID, ItemDTO item)
        {
            Item newItem = new Item(item.quantity, item.itemName, item.description, item.price, item.category);
            return userManagerFacade.editItemDetails(userName, storeID, newItem);
        }
        public bool appointStoreManager(String userName, String managerName, int storeID)
        {
            return userManagerFacade.appointStoreManager(userName, managerName, storeID);
        }
        public bool appointStoreOwner(String userName, String storeOwnerName, int storeID)
        {
            return userManagerFacade.appointStoreOwner(userName, storeOwnerName, storeID);
        }
        public bool editManagerPermissions(String userName, String managerName, ConcurrentBag<int> permissions)
        {
            ConcurrentBag<Permissions> newPermissions = new ConcurrentBag<Permissions>();
            foreach (int i in permissions)
            {
                Permissions permission = (Permissions)i;
                newPermissions.Add(permission);
            }
            return userManagerFacade.editManagerPermissions(userName, managerName, newPermissions);
        }
        public bool removeStoreManager(String userName, String managerName, int storeID)
        {
            return userManagerFacade.removeStoreManager(userName, managerName, storeID);
        }
        public ConcurrentDictionary<String, ConcurrentBag<Permissions>> getOfficialsInformation(String userName, int storeID)
        {
            return userManagerFacade.getOfficialsInformation(userName, storeID);
        }
        public ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(String userName, int storeID)
        {
            return userManagerFacade.getStorePurchaseHistory(userName, storeID);
        }
        public ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory()
        {
            return userManagerFacade.getUsersPurchaseHistory();
        }
        public ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory()
        {
            return userManagerFacade.getStoresPurchaseHistory();
        }

    }
}
