using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using WSEP212.DomainLayer;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;
using WSEP212.ServiceLayer.ServiceObjectsDTO;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.SystemLoggers;
using WSEP212.DomainLayer.PurchasePolicy;
using WSEP212.DomainLayer.SalePolicy;
using WSEP212.DomainLayer.SalePolicy.SaleOn;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;

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
                //UserRepository.Instance.createSystemManager();
            }
            catch (SystemException e)
            {
                Logger.Instance.writeErrorEventToLog(e.Message);
            }
        }

        public void initRepo()
        {
            try
            {
                UserRepository.Instance.initRepo();
                //UserRepository.Instance.createSystemManager();
                UserRepository.Instance.Init();
                //StoreRepository.Instance.Init();
                SystemController.Instance.Init();
            }
            catch (SystemException e)
            {
                Logger.Instance.writeErrorEventToLog(e.Message);
            }
        }

        public void Init()
        {
            string jsonFilePath = "init.json";
            string json = File.ReadAllText(jsonFilePath);
            dynamic array = JsonConvert.DeserializeObject(json);

            // CREATE STORES
            foreach (var item in array.stores)
            {
                string storeOpener = item.storeOpener;
                string storeName = item.storeName;
                string storeAddress = item.storeAddress;
                openStore(storeOpener, storeName, storeAddress, "0", "0");
            }

            // ADD ITEMS
            foreach (var item in array.items)
            {
                string userAdded = item.userAdded;
                string storeName = item.storeName;
                string storeID = item.storeID;
                string itemName = item.itemName;
                string itemPrice = item.itemPrice;
                string itemQuantity = item.itemQuantity;
                string description = item.description;
                string category = item.category;
                addItemToStorage(userAdded, int.Parse(storeID),
                    new ItemDTO(int.Parse(storeID), int.Parse(itemQuantity), itemName, description,
                        new ConcurrentDictionary<string, ItemReview>(), double.Parse(itemPrice), int.Parse(category)));
            }

            // APPOINT
            foreach (var item in array.appoints)
            {
                string manager = item.manager;
                string appoint = item.appoint;
                string storeName = item.storeName;
                string storeID = item.storeID;
                appointStoreManager(manager, appoint, int.Parse(storeID));
                ConcurrentLinkedList<int> perms = new ConcurrentLinkedList<int>();
                foreach (var perm in item.permissions)
                {
                    if (perm.ToString().Equals("StorageManagment"))
                        perms.TryAdd((int)Permissions.StorageManagment);
                }
                editManagerPermissions(manager, appoint, perms, int.Parse(storeID));
            }

            UserRepository.Instance.initRepo();
            string loggedUser = array.loggedUser;
            logout(loggedUser);
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

        public ResultWithValue<NotificationDTO> addItemToShoppingCart(String userName, int storeID, int itemID,
            int quantity, Int32 purchaseType, double startPrice)
        {
            String info = $"AddItemToShoppingCart Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, item ID: {itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            ResultWithValue<ConcurrentLinkedList<string>> res =
                SystemControllerFacade.Instance.addItemToShoppingCart(userName, storeID, itemID, quantity,
                    (PurchaseType) purchaseType, startPrice);
            return res.getTag()
                ? new OkWithValue<NotificationDTO>(res.getMessage(),
                    new NotificationDTO(res.getValue(),
                        $"The user: {userName}, submit new price offer for item: {itemID}; with price: {startPrice}!\n please review this offer"))
                : new FailureWithValue<NotificationDTO>(res.getMessage(), null);
        }

        public RegularResult removeItemFromShoppingCart(String userName, int storeID, int itemID)
        {
            String info = $"RemoveItemFromShoppingCart Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, item ID: {itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.removeItemFromShoppingCart(userName, storeID, itemID);
        }

        public ResultWithValue<NotificationDTO> purchaseItems(String userName,
            DeliveryParametersDTO deliveryParametersDTO, PaymentParametersDTO paymentParametersDTO)
        {
            String info = $"PurchaseItems Event was triggered, with the parameter:" +
                          $"user name: {userName}, delivery Params: {deliveryParametersDTO}, payment Params: {paymentParametersDTO}";
            Logger.Instance.writeInformationEventToLog(info);
            DeliveryParameters deliveryParameters = new DeliveryParameters(deliveryParametersDTO);
            PaymentParameters paymentParameters = new PaymentParameters(paymentParametersDTO);
            var usersToSendRes =
                SystemControllerFacade.Instance.purchaseItems(userName, deliveryParameters, paymentParameters);
            return usersToSendRes.getTag()
                ? new OkWithValue<NotificationDTO>(usersToSendRes.getMessage(),
                    new NotificationDTO(usersToSendRes.getValue(),
                        $"The user {userName} has purchased your item"))
                : new FailureWithValue<NotificationDTO>(usersToSendRes.getMessage(), null);
        }

        public ResultWithValue<int> openStore(String userName, String storeName, String storeAddress,
            String purchasePolicy, String salesPolicy)
        {
            String info = $"OpenStore Event was triggered, with the parameter:" +
                          $"user name: {userName}, store name: {storeName}, purchase policy: {purchasePolicy}, sales policy: {salesPolicy}";
            Logger.Instance.writeInformationEventToLog(info);
            PurchasePolicy newPurchasePolicy = new PurchasePolicy(purchasePolicy);
            SalePolicy newSalesPolicy = new SalePolicy(salesPolicy);
            return SystemControllerFacade.Instance.openStore(userName, storeName, storeAddress, newPurchasePolicy,
                newSalesPolicy);
        }

        public ConcurrentLinkedList<PurchaseType> getStorePurchaseTypes(string userName, int storeID)
        {
            return StoreRepository.Instance.stores[storeID].purchasePolicy.purchaseTypes;
        }

        public ResultWithValue<NotificationDTO> itemReview(String userName, String review, int itemID, int storeID)
        {
            String info = $"ItemReview Event was triggered, with the parameters:" +
                          $"user name: {userName}, review: {review}, store ID: {storeID}, item ID: {itemID}";
            Logger.Instance.writeInformationEventToLog(info);
            var usersToSendRes = SystemControllerFacade.Instance.itemReview(userName, review, itemID, storeID);
            return usersToSendRes.getTag()
                ? new OkWithValue<NotificationDTO>(usersToSendRes.getMessage(),
                    new NotificationDTO(usersToSendRes.getValue(),
                        $"The user {userName} has reviewed your item (ID: {itemID} in StoreID: {storeID})"))
                : new FailureWithValue<NotificationDTO>(usersToSendRes.getMessage(), null);
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
            return SystemControllerFacade.Instance.addItemToStorage(userName, storeID, item.quantity, item.itemName,
                item.description, item.price, (ItemCategory) item.category);
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
            return SystemControllerFacade.Instance.editItemDetails(userName, storeID, item.itemID, item.quantity,
                item.itemName, item.description, item.price, (ItemCategory) item.category);
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

        public RegularResult editManagerPermissions(String userName, String managerName,
            ConcurrentLinkedList<Int32> permissions, int storeID)
        {
            String permissionsStr = "permissions: ";

            ConcurrentLinkedList<Permissions> newPermissions = new ConcurrentLinkedList<Permissions>();
            Node<Int32> permission = permissions.First;
            while (permission.Next != null)
            {
                permissionsStr += $"{permission.Value}, ";
                newPermissions.TryAdd((Permissions) permission.Value);
                permission = permission.Next;
            }

            String info = $"EditManagerPermissions Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, manager name: {managerName}," +
                          $"{permissionsStr}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.editManagerPermissions(userName, managerName, newPermissions,
                storeID);
        }

        public ResultWithValue<NotificationDTO> removeStoreManager(String userName, String managerName, int storeID)
        {
            String info = $"RemoveStoreManager Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, manager name: {managerName}";
            Logger.Instance.writeInformationEventToLog(info);
            var res = SystemControllerFacade.Instance.removeStoreManager(userName, managerName, storeID);
            return res.getTag()
                ? new OkWithValue<NotificationDTO>(res.getMessage(),
                    new NotificationDTO(null, $"The user {userName} has fired you! You are no longer store owner!"))
                : new FailureWithValue<NotificationDTO>(res.getMessage(), null);
        }

        public ResultWithValue<NotificationDTO> removeStoreOwner(String userName, String ownerName, int storeID)
        {
            String info = $"RemoveStoreManager Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}, manager name: {ownerName}";
            Logger.Instance.writeInformationEventToLog(info);
            var res = SystemControllerFacade.Instance.removeStoreOwner(userName, ownerName, storeID);
            return res.getTag()
                ? new OkWithValue<NotificationDTO>(res.getMessage(),
                    new NotificationDTO(null, $"The user {userName} has fired you! You are no longer store owner!"))
                : new FailureWithValue<NotificationDTO>(res.getMessage(), null);
        }

        public ResultWithValue<ConcurrentDictionary<String, ConcurrentLinkedList<Permissions>>> getOfficialsInformation(
            String userName, int storeID)
        {
            String info = $"GetOfficialsInformation Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getOfficialsInformation(userName, storeID);
        }

        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getStorePurchaseHistory(String userName,
            int storeID)
        {
            String info = $"GetStorePurchaseHistory Event was triggered, with the parameters:" +
                          $"user name: {userName}, store ID: {storeID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getStorePurchaseHistory(userName, storeID);
        }

        public ResultWithValue<ConcurrentDictionary<String, ConcurrentDictionary<int, PurchaseInvoice>>>
            getUsersPurchaseHistory(
                String userName)
        {
            String info = $"GetUsersPurchaseHistory Event was triggered, with the parameter:" +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getUsersPurchaseHistory(userName);
        }

        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, PurchaseInvoice>>>
            getStoresPurchaseHistory(
                String userName)
        {
            String info = $"GetStoresPurchaseHistory Event was triggered, with the parameter:" +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getStoresPurchaseHistory(userName);
        }

        public ResultWithValue<ConcurrentDictionary<int, PurchaseInvoice>> getUserPurchaseHistory(String userName)
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

        public ConcurrentDictionary<Item, int> searchItems(String itemName = "", String keyWords = "",
            double minPrice = Double.MinValue, double maxPrice = Double.MaxValue, Int32 category = 0)
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

        public RegularResult addBidOffer(String userName, int storeID, int itemID, string buyer, double offerItemPrice)
        {
            String info = $"addBidOffer Event was triggered, with the parameter: " +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.addBidOffer(userName, storeID, itemID, buyer, offerItemPrice);
        }
        
        public RegularResult removeBidOffer(String userName, int storeID, int itemID, string buyer)
        {
            String info = $"removeBidOffer Event was triggered, with the parameter: " +
                          $"user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.removeBidOffer(userName, storeID, itemID, buyer);
        }

        public ResultWithValue<int> addPurchasePredicate(string userName, int storeID,
            LocalPredicate<PurchaseDetails> newPredicate, String predDescription)
        {
            String info = $"addPurchasePredicate Event was triggered, with the parameter: " +
                          $"user name: {userName}, storeID: {storeID}, newPredicate: {newPredicate}, predDescription: {predDescription}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.addPurchasePredicate(userName, storeID, newPredicate,
                predDescription);
        }

        public RegularResult removePurchasePredicate(string userName, int storeID, int predicateID)
        {
            String info = $"removePurchasePredicate Event was triggered, with the parameter: " +
                          $"user name: {userName}, storeID: {storeID}, predicateID: {predicateID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.removePurchasePredicate(userName, storeID, predicateID);
        }

        public ResultWithValue<int> composePurchasePredicates(string userName, int storeID, int firstPredicateID,
            int secondPredicateID, int typeOfComposition)
        {
            String info = $"composePurchasePredicates Event was triggered, with the parameter: " +
                          $"user name: {userName}, storeID: {storeID}, firstPredicateID: {firstPredicateID}, secondPredicateID: {secondPredicateID}, typeOfComposition: {typeOfComposition}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.composePurchasePredicates(userName, storeID, firstPredicateID,
                secondPredicateID, (PurchasePredicateCompositionType) typeOfComposition);
        }

        public ResultWithValue<int> addSale(string userName, int storeID, int salePercentage, ApplySaleOn saleOn,
            String saleDescription)
        {
            String info = $"addSale Event was triggered, with the parameter: " +
                          $"user name: {userName}, storeID: {storeID}, salePercentage: {salePercentage}, saleOn: {saleOn}, saleDescription: {saleDescription}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.addSale(userName, storeID, salePercentage, saleOn, saleDescription);
        }

        public RegularResult removeSale(string userName, int storeID, int saleID)
        {
            String info = $"removeSale Event was triggered, with the parameter: " +
                          $"user name: {userName}, storeID: {storeID}, saleID: {saleID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.removeSale(userName, storeID, saleID);
        }

        public ResultWithValue<int> addSaleCondition(string userName, int storeID, int saleID,
            SimplePredicate condition, int compositionType)
        {
            String info = $"addSaleCondition Event was triggered, with the parameter: " +
                          $"user name: {userName}, storeID: {storeID}, saleID: {saleID}, condition: {condition}, compositionType: {compositionType}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.addSaleCondition(userName, storeID, saleID, condition,
                (SalePredicateCompositionType) compositionType);
        }

        public ResultWithValue<int> composeSales(string userName, int storeID, int firstSaleID, int secondSaleID,
            int typeOfComposition, SimplePredicate selectionRule)
        {
            String info = $"composeSales Event was triggered, with the parameter: " +
                          $"user name: {userName}, storeID: {storeID}, firstSaleID: {firstSaleID}, secondSaleID: {secondSaleID}, typeOfComposition: {typeOfComposition}, selectionRule: {selectionRule}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.composeSales(userName, storeID, firstSaleID, secondSaleID,
                (SaleCompositionType) typeOfComposition, selectionRule);
        }

        public ResultWithValue<ConcurrentLinkedList<int>> getUsersStores(String userName)
        {
            String info = $"Get user's stores Event was triggered, with the parameter: user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getUsersStores(userName);
        }

        public RegularResult isStoreOwner(string userName, int storeID)
        {
            ResultWithValue<SellerPermissions> pers = StoreRepository.Instance.stores[storeID]
                .getStoreSellerPermissions(userName);
            if (pers.getTag())
            {
                if (pers.getValue().permissionsInStore.Contains(Permissions.AllPermissions))
                {
                    return new Ok("the user is a store owner");
                }

                return new Failure("the user is not a store owner!");
            }

            return new Failure("the user is not a store owner!");
        }

        private Permissions[] listToArray(ConcurrentLinkedList<Permissions> lst)
        {
            Permissions[] arr = new Permissions[lst.size];
            int i = 0;
            Node<Permissions> node = lst.First; // going over the user's permissions to check if he is a store manager or owner
            while(node.Next != null)
            {
                arr[i] = node.Value;
                node = node.Next;
                i++;
            }
            return arr;
        }
        
        public RegularResult hasPermission(string userName, int storeID, Permissions permission)
        {
            ResultWithValue<SellerPermissions> pers = StoreRepository.Instance.stores[storeID]
                .getStoreSellerPermissions(userName);
            if (pers.getTag())
            {
                var arrPer = listToArray(pers.getValue().permissionsInStore);
                if (arrPer.Contains(permission) || arrPer.Contains(Permissions.AllPermissions))
                {
                    return new Ok("the user has this permission");
                }

                return new Failure("the user does not have this permission!");
            }
            return new Failure("the user does not have this permission!");
        }
        
        public string[] getAllSignedUpUsers()
        {
            return Authentication.Instance.getAllUsers(); //TODO: change?
        }

        public Store getStoreByID(int storeID)
        {
            return StoreRepository.Instance.stores[storeID];
        }

        public KeyValuePair<Item,int> getItemByID(int itemID)
        {
            return StoreRepository.Instance.getItemByID(itemID);
        }
        
        public RegularResult continueAsGuest(String userName)
        {
            String info = $"Continue As Guest Event was triggered, with the parameter: user name: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.continueAsGuest(userName);
        }

        public ResultWithValue<ConcurrentDictionary<int, string>> getStorePredicatesDescription(int storeID)
        {
            String info = $"getStorePredicatesDescription Event was triggered, with the parameters:" +
                          $"store ID: {storeID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getStorePredicatesDescription(storeID);
        }

        public ResultWithValue<ConcurrentDictionary<int, string>> getStoreSalesDescription(int storeID)
        {
            String info = $"getStoreSalesDescription Event was triggered, with the parameters:" +
                          $"store ID: {storeID}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getStoreSalesDescription(storeID);
        }

        public RegularResult changeItemQuantityInShoppingCart(string userName, int storeID, int itemID, int updatedQuantity)
        {
            String info = $"changeItemQuantityInShoppingCart Event was triggered, with the parameters:" +
                          $"userName: {userName}, store ID: {storeID}, item ID: {itemID}, quantity: {updatedQuantity}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.changeItemQuantityInShoppingCart(userName, storeID, itemID, updatedQuantity);
        }

        public RegularResult changeItemPurchaseType(string userName, int storeID, int itemID, int purchaseType, double startPrice)
        {
            String info = $"changeItemPurchaseType Event was triggered, with the parameters:" +
                          $"userName: {userName}, store ID: {storeID}, item ID: {itemID}, purchase Type: {purchaseType}, price: {startPrice}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.changeItemPurchaseType(userName, storeID, itemID, (PurchaseType)purchaseType, startPrice);
        }

        public ResultWithValue<NotificationDTO> submitPriceOffer(string userName, int storeID, int itemID, double offerItemPrice)
        {
            String info = $"submitPriceOffer Event was triggered, with the parameters:" +
                          $"userName: {userName}, store ID: {storeID}, item ID: {itemID}, offer price: {offerItemPrice}";
            Logger.Instance.writeInformationEventToLog(info);
            ResultWithValue<ConcurrentLinkedList<string>> res = SystemControllerFacade.Instance.submitPriceOffer(userName, storeID, itemID, offerItemPrice);
            return res.getTag() ? new OkWithValue<NotificationDTO>(res.getMessage(),
                new NotificationDTO(res.getValue(), $"The user {userName} submit new price offer fot item {itemID}.\n please review this offer")) :
                new FailureWithValue<NotificationDTO>(res.getMessage(), null);
        }

        public ResultWithValue<NotificationDTO> confirmPriceStatus(string storeManager, string userToConfirm, int storeID, int itemID, int priceStatus)
        {
            String info = $"confirmPriceStatus Event was triggered, with the parameters:" +
                          $"store Manager: {storeManager}, user To Confirm: {userToConfirm}, store ID: {storeID}, item ID: {itemID}, price Status: {priceStatus}";
            Logger.Instance.writeInformationEventToLog(info);
            ResultWithValue<string> res = SystemControllerFacade.Instance.confirmPriceStatus(storeManager, userToConfirm, storeID, itemID, (PriceStatus)priceStatus);
            if(res.getTag())
            {
                ConcurrentLinkedList<string> userToSend = new ConcurrentLinkedList<string>();
                userToSend.TryAdd(res.getValue());
                string decision = priceStatus == 0 ? "approve" : "reject";
                return new OkWithValue<NotificationDTO>(res.getMessage(),
                    new NotificationDTO(userToSend, $"Store manager {storeManager} review your offer on item {itemID}.\n he decided to {decision} your offer."));
            }
            return new FailureWithValue<NotificationDTO>(res.getMessage(), null);
        }

        public ResultWithValue<NotificationDTO> itemCounterOffer(string storeManager, string userToConfirm, int storeID, int itemID, double counterOffer)
        {
            String info = $"itemCounterOffer Event was triggered, with the parameters:" +
                          $"storeManager: {storeManager}, userName: {userToConfirm}, store ID: {storeID}, item ID: {itemID}, counter offer: {counterOffer}";
            Logger.Instance.writeInformationEventToLog(info);
            ResultWithValue<string> res = SystemControllerFacade.Instance.itemCounterOffer(storeManager, userToConfirm, storeID, itemID, counterOffer);
            if (res.getTag())
            {
                ConcurrentLinkedList<string> userToSend = new ConcurrentLinkedList<string>();
                userToSend.TryAdd(res.getValue());
                return new OkWithValue<NotificationDTO>(res.getMessage(),
                    new NotificationDTO(userToSend, $"Store manager {storeManager} review your offer on item {itemID}.\n he decided to counter your offer to {counterOffer}."));
            }
            return new FailureWithValue<NotificationDTO>(res.getMessage(), null);
        }

        public RegularResult supportPurchaseType(string userName, int storeID, int purchaseType)
        {
            String info = $"supportPurchaseType Event was triggered, with the parameters:" +
                          $"userName: {userName}, store ID: {storeID}, purchase type: {purchaseType}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.supportPurchaseType(userName, storeID, (PurchaseType)purchaseType);
        }

        public RegularResult unsupportPurchaseType(string userName, int storeID, int purchaseType)
        {
            String info = $"unsupportPurchaseType Event was triggered, with the parameters:" +
                          $"userName: {userName}, store ID: {storeID}, purchase type: {purchaseType}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.unsupportPurchaseType(userName, storeID, (PurchaseType)purchaseType);
        }

        public ResultWithValue<ConcurrentDictionary<int, ConcurrentDictionary<int, KeyValuePair<double, PriceStatus>>>> getItemsAfterSalePrices(String userName)
        {
            String info = $"getItemsAfterSalePrices Event was triggered, with the parameters:" +
                          $"userName: {userName}";
            Logger.Instance.writeInformationEventToLog(info);
            return SystemControllerFacade.Instance.getItemsAfterSalePrices(userName);
        }
    }
}
