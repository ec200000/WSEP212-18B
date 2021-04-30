using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using WSEP212.DomainLayer;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;

namespace WSEP212.ServiceLayer
{
    public class SystemController : ISystemController
    {
        private static readonly Lazy<SystemController> lazy
            = new Lazy<SystemController>(() => new SystemController());

        public static SystemController Instance
            => lazy.Value;

        private SystemController()
        {
            try
            {
                UserRepository.Instance.createSystemManager();
            }
            catch (SystemException e)
            {
                Logger.Instance.writeErrorEventToLog(e.Message);
            }
        }
        
        public RegularResult register(String userName, int userAge, String password)
        {
            String info = $"Register Event was triggered, with the parameters: " +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.register(userName, userAge, password);
        }

        public RegularResult login(String userName, String password)
        {
            String info = $"Login Event was triggered, with the parameters: " +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.login(userName, password);
        }
        public RegularResult logout(String userName)
        {
            String info = $"Logout Event was triggered, with the parameter: user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.logout(userName);
        }

        public RegularResult addItemToShoppingCart(String userName, int storeID, int itemID, int quantity)
        {
            String info = $"AddItemToShoppingCart Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, item ID: {itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.addItemToShoppingCart(userName, storeID, itemID, quantity);
        }

        public RegularResult removeItemFromShoppingCart(String userName, int storeID, int itemID)
        {
            String info = $"RemoveItemFromShoppingCart Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, item ID: {itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.removeItemFromShoppingCart(userName, storeID, itemID);
        }

        public RegularResult purchaseItems(String userName, String address)
        {
            String info = $"PurchaseItems Event was triggered, with the parameter:" +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.purchaseItems(userName, address);
        }

        public ResultWithValue<int> openStore(String userName, String storeName, String storeAddress, String purchasePolicy, String salesPolicy)
        {
            String info = $"OpenStore Event was triggered, with the parameter:" +
                          $"user name: {userName}, store name: {storeName}, purchase policy: {purchasePolicy}, sales policy: {salesPolicy}";
            Logger.Instance.writeInformationEventToLog(info);
            ConcurrentLinkedList<PurchaseType> purchaseRoutes = new ConcurrentLinkedList<PurchaseType>();
            purchaseRoutes.TryAdd(PurchaseType.ImmediatePurchase);
            PurchasePolicy newPurchasePolicy = new PurchasePolicy(purchasePolicy, new ConcurrentLinkedList<PolicyPredicate>());
            SalePolicy newSalesPolicy = new SalePolicy(salesPolicy);
            return SystemControllerFacade.Instance.openStore(userName, storeName, storeAddress, newPurchasePolicy, newSalesPolicy);
        }

        public RegularResult itemReview(String userName, String review, int itemID, int storeID)
        {
            String info = $"ItemReview Event was triggered, with the parameters:" +
                          $"user name: {userName}, review: {review}, store ID: {storeID}, item ID: {itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.itemReview(userName, review, itemID, storeID);
        }

        public ResultWithValue<int> addItemToStorage(String userName, int storeID, ItemDTO item)
        {
            if (item == null)
            {
                return new FailureWithValue<int>("Item is null", -1);
            }
            String info = $"AddItemToStorage Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, item ID: {item.itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.addItemToStorage(userName, storeID, item.quantity, item.itemName, item.description, item.price, item.category);
        }

        public RegularResult removeItemFromStorage(String userName, int storeID, int itemID)
        {
            String info = $"RemoveItemFromStorage Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, item ID: {itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.removeItemFromStorage(userName, storeID, itemID);
        }

        public RegularResult editItemDetails(String userName, int storeID, ItemDTO item)
        {
            if (item == null)
            {
                return new Failure("Item is null");
            }
            String info = $"EditItemDetails Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, item ID: {item.itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.editItemDetails(userName, storeID, item.itemID, item.quantity, item.itemName, item.description, item.price, item.category);
        }

        public RegularResult appointStoreManager(String userName, String managerName, int storeID)
        {
            String info = $"AppointStoreManager Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, manager name: {managerName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.appointStoreManager(userName, managerName, storeID);
        }

        public RegularResult appointStoreOwner(String userName, String storeOwnerName, int storeID)
        {
            String info = $"AppointStoreManager Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, store owner name: {storeOwnerName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.appointStoreOwner(userName, storeOwnerName, storeID);
        }

        public RegularResult editManagerPermissions(String userName, String managerName, ConcurrentLinkedList<Int32> permissions, int storeID)
        {
            String permissionsStr = "permissions: ";
            
            ConcurrentLinkedList<Permissions> newPermissions = new ConcurrentLinkedList<Permissions>();
            Node<Int32> permission = permissions.First;
            while(permission.Next != null)
            {
                permissionsStr += $"{permission.Value}, ";
                newPermissions.TryAdd((Permissions)permission.Value);
                permission = permission.Next;
            }
            String info = $"EditManagerPermissions Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, manager name: {managerName}," +
                          $"{permissionsStr}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.editManagerPermissions(userName, managerName, newPermissions, storeID);
        }

        public RegularResult removeStoreManager(String userName, String managerName, int storeID)
        {
            String info = $"RemoveStoreManager Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, manager name: {managerName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.removeStoreManager(userName, managerName, storeID);
        }
        
        public RegularResult removeStoreOwner(String userName, String ownerName, int storeID)
        {
            String info = $"RemoveStoreManager Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, manager name: {ownerName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.removeStoreManager(userName, ownerName, storeID);
        }

        public ResultWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>> getOfficialsInformation(String userName, int storeID)
        {
            String info = $"GetOfficialsInformation Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getOfficialsInformation(userName, storeID);
        }

        public ResultWithValue<ConcurrentBag<PurchaseInvoice>> getStorePurchaseHistory(String userName, int storeID)
        {
            String info = $"GetStorePurchaseHistory Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getStorePurchaseHistory(userName, storeID);
        }

        public ResultWithValue<ConcurrentDictionary<String, ConcurrentBag<PurchaseInvoice>>> getUsersPurchaseHistory(String userName)
        {
            String info = $"GetUsersPurchaseHistory Event was triggered, with the parameter:" +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getUsersPurchaseHistory(userName);
        }

        public ResultWithValue<ConcurrentDictionary<int, ConcurrentBag<PurchaseInvoice>>> getStoresPurchaseHistory(String userName)
        {
            String info = $"GetStoresPurchaseHistory Event was triggered, with the parameter:" +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getStoresPurchaseHistory(userName);
        }

        public ResultWithValue<ConcurrentBag<PurchaseInvoice>> getUserPurchaseHistory(String userName)
        {
            String info = $"GetUserPurchaseHistory Event was triggered, with the parameter:" +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getUserPurchaseHistory(userName);
        }
        
        public ConcurrentDictionary<Store, ConcurrentLinkedList<Item>> getItemsInStoresInformation() 
        {
            String info = $"GetItemsInStoresInformation Event was triggered"; 
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getItemsInStoresInformation();
        }

        public ConcurrentDictionary<Item, int> searchItems(String itemName = "", String keyWords = "", double minPrice = Double.MinValue, double maxPrice = Double.MaxValue, String category = "")
        {
            String info = $"SearchItemsByCategory Event was triggered, with the parameters:" +
                          $"item name: {itemName}, key words: {keyWords}, meminimal price: {minPrice}, maximal price: {maxPrice}, category: {category}"; 
            Logger.Instance.writeInformationEventToLog(info);
            SearchItemsDTO searchItemsDTO = new SearchItemsDTO(itemName, keyWords, minPrice, maxPrice, category);
            return SystemControllerFacade.Instance.searchItems(searchItemsDTO);
        }

        public RegularResult loginAsSystemManager(string userName, string password)
        {
            String info = $"LoginAsSystemManager Event was triggered, with the parameters: " +
                          $"user name: {userName}, password: {password} ";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.loginAsSystemManager(userName, password);
        }

        public ResultWithValue<ShoppingCart> viewShoppingCart(string userName)
        {
            String info = $"ViewShoppingCart Event was triggered, with the parameter: " +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.viewShoppingCart(userName);
        }
        
        public ResultWithValue<ConcurrentLinkedList<int>> getUsersStores(String userName)
        {
            String info = $"Get user's stores Event was triggered, with the parameter: user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getUsersStores(userName);
        }

        public RegularResult continueAsGuest(String userName)
        {
            String info = $"Continue As Guest Event was triggered, with the parameter: user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.continueAsGuest(userName);
        }
    }
}
