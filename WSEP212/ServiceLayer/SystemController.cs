using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using WSEP212.DomainLayer;

namespace WSEP212.ServiceLayer
{
    public class SystemController : ISystemController
    {
        public SystemController() { }

        public bool register(String userName, String password)
        {
            return SystemControllerFacade.Instance.register(userName, password);
        }
        public bool login(String userName, String password)
        {
            return SystemControllerFacade.Instance.login(userName, password);
        }
        public bool logout(String userName)
        {
            return SystemControllerFacade.Instance.logout(userName);
        }

        public bool addItemToShoppingCart(String userName, int storeID, int itemID)
        {
            return SystemControllerFacade.Instance.addItemToShoppingCart(userName, storeID, itemID);
        }
        public bool removeItemFromShoppingCart(String userName, int storeID, int itemID)
        {
            return SystemControllerFacade.Instance.removeItemFromShoppingCart(userName, storeID, itemID);
        }

        public bool purchaseItems(String userName)
        {
            return SystemControllerFacade.Instance.purchaseItems(userName);
        }
        public bool openStore(String userName, String storeName, String purchasePolicy, String salesPolicy)
        {
            PurchasePolicy newPurchasePolicy = new PurchasePolicy(purchasePolicy, null, null);
            SalesPolicy newSalesPolicy = new SalesPolicy(salesPolicy, null);
            return SystemControllerFacade.Instance.openStore(userName, storeName, newPurchasePolicy, newSalesPolicy);
        }
        public bool itemReview(String userName, String review, int itemID, int storeID)
        {
            return SystemControllerFacade.Instance.itemReview(userName, review, itemID, storeID);
        }
        public bool addItemToStorage(String userName, int storeID, ItemDTO item)
        {
            return SystemControllerFacade.Instance.addItemToStorage(userName, storeID, item.quantity, item.itemName, item.description, item.price, item.category);
        }
        public bool removeItemFromStorage(String userName, int storeID, int itemID)
        {
            return SystemControllerFacade.Instance.removeItemFromStorage(userName, storeID, itemID);
        }
        public bool editItemDetails(String userName, int storeID, ItemDTO item)
        {
            Item newItem = new Item(item.quantity, item.itemName, item.description, item.price, item.category);
            return SystemControllerFacade.Instance.editItemDetails(userName, storeID, newItem);
        }
        public bool appointStoreManager(String userName, String managerName, int storeID)
        {
            return SystemControllerFacade.Instance.appointStoreManager(userName, managerName, storeID);
        }
        public bool appointStoreOwner(String userName, String storeOwnerName, int storeID)
        {
            return SystemControllerFacade.Instance.appointStoreOwner(userName, storeOwnerName, storeID);
        }
        public bool editManagerPermissions(String userName, String managerName, ConcurrentBag<int> permissions, int storeID)
        {
            ConcurrentBag<Permissions> newPermissions = new ConcurrentBag<Permissions>();
            foreach (int i in permissions)
            {
                Permissions permission = (Permissions)i;
                newPermissions.Add(permission);
            }
            return SystemControllerFacade.Instance.editManagerPermissions(userName, managerName, newPermissions, storeID);
        }
        public bool removeStoreManager(String userName, String managerName, int storeID)
        {
            return SystemControllerFacade.Instance.removeStoreManager(userName, managerName, storeID);
        }
        public ConcurrentDictionary<String, ConcurrentBag<Permissions>> getOfficialsInformation(String userName, int storeID)
        {
            return SystemControllerFacade.Instance.getOfficialsInformation(userName, storeID);
        }
        public ConcurrentBag<PurchaseInfo> getStorePurchaseHistory(String userName, int storeID)
        {
            return SystemControllerFacade.Instance.getStorePurchaseHistory(userName, storeID);
        }
        public ConcurrentDictionary<String, ConcurrentBag<PurchaseInfo>> getUsersPurchaseHistory(String userName)
        {
            return SystemControllerFacade.Instance.getUsersPurchaseHistory(userName);
        }
        public ConcurrentDictionary<int, ConcurrentBag<PurchaseInfo>> getStoresPurchaseHistory(String userName)
        {
            return SystemControllerFacade.Instance.getStoresPurchaseHistory(userName);
        }

    }
}
